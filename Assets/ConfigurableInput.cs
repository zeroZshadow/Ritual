using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;

/// <summary>
/// Configurable input.
/// 
/// This class was made to create in-game mappable input for Unity3D. 
/// It relies heavily on the Axes defined in the project's input settings, so make sure to use the provided InputManager.asset file. 
/// This file contains the mapping for every joystick + axis that I felt safe to assume people are limited to. (5 Joysticks with 10 axes each). 
/// Changing these project settings should not be needed. 
/// 
/// Every keybinding can have a primary and secondary mapping, aswell as a primary and secondary modifier. 
/// Modifiers can also be used as separate key, however be adviced that their mapped actions will be registered even if they are used as modifier.
/// Keys that can be used as modifiers during scanning are automatically set when setting keys or axes using those modifiers. 
/// You can also set modifiers for scanning yourself by calling the public SetModifier method. 
/// 
/// </summary>

public class ConfigurableInput : MonoBehaviour {
	
	/// <summary>
	/// Input map entries are how key mappings are defined and keep some info about wether or not any key events are registered.
	/// </summary>
	private class InputMapEntry{
		public string Name;
		public bool KeyDown; 
		public bool KeyUp;
		public bool KeyHeld;
		public KeyCode PrimaryKey = KeyCode.None;
		public KeyCode SecondaryKey = KeyCode.None; 
		public KeyCode PrimaryModifier = KeyCode.None;
		public KeyCode SecondaryModifier = KeyCode.None; 
		public bool PrimaryIsAxis = false;
		public float PrimaryAxisValue = 0;
		public string PrimaryAxisName = "";
		public float PrimaryAxisDeadzone = 0.2f;
		public bool SecondaryIsAxis = false;
		public float SecondaryAxisValue = 0;
		public string SecondaryAxisName = "";
		public float SecondaryAxisDeadzone = 0.2f;
	}
	
	/// <summary>
	/// Axis entries are used for getting positive/negative axis values through the GetAxis method.
	/// </summary>
	private class InputAxisEntry{
		public string Name;
		public string KeyPositive;
		public string KeyNegative;
	}
	
	public static ConfigurableInput Instance;
	
	public delegate void KeyScanned (KeyCode key, KeyCode modiefier);
	public delegate void AxisScanned (string axis, KeyCode modiefier);
	
	/// <summary>
	/// Occurs when a key is scanned.
	/// </summary>
	public event KeyScanned OnKeyScanned;
	/// <summary>
	/// Occurs when an axis is scanned.
	/// </summary>
	public event AxisScanned OnAxisScanned; 
	
	private Dictionary<string, InputMapEntry> MapEntries;
	private Dictionary<string, InputAxisEntry> AxisEntries;
	private Dictionary<KeyCode, int> Modifiers; //For saving the state of modifier keys each frame
	private List<string> ModifiedAxes; //For checking what axes are modified so we can ignore unmodified input when modifiers are pressed. 
	private List<KeyCode> ModifiedKeys; //For checking what keys are modified so we can ignore unmodified input when modifiers are pressed. 
	private List<KeyCode> ModifierKeys; //For checking what keys are used as modifiers
	private bool ModifierPressed; 
	private bool Scanning = false;
	
	private const int NumberOfJoysticks = 5; //Unity doesn't support more joysticks.
	private const int NumberOfAxes = 10; //We assume no joystick has more than 10 axes.
	private string[,] AxesStrings; //Cache For keeping axis names that are defined in the project's input settings. 
	
	int NrOfKeyCodes;
	
	//Keeps previous axis values for checking axis differences while scanning for input. 
	//This prevents axes that continuously give input (like throttles) from constantly being registered. 
	private float[,] AxesPrevValues; 
	
	void Awake(){
		NrOfKeyCodes = Enum.GetValues(typeof(KeyCode)).Length;
		//NrOfKeyCodes = 450;
		if (Instance == null) {
			Instance = this;
		}else{
			throw new System.InvalidOperationException("Can not have multiple instances of ConfigurableInput");
		}
		MapEntries = new Dictionary<string, InputMapEntry>();
		Modifiers = new Dictionary<KeyCode, int>();
		ModifiedKeys = new List<KeyCode>();
        ModifierKeys = new List<KeyCode>();
		ModifiedAxes = new List<string>();
		AxisEntries = new Dictionary<string, InputAxisEntry>();
		InitAxes();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ModifierPressed = false;
		
		//Register which modifiers are pressed.
		foreach(KeyCode Modifier in ModifierKeys){
			CheckModifier(Modifier);
		}
		
		//Register the mapped key states
		foreach(InputMapEntry MapEntry in MapEntries.Values){
			CheckEntryInput(MapEntry);
		}
		
		if (Scanning){
			//Register any kind of input, usefull for input mapping from a GUI.
			ScanInput();
		}
	}
	
	/// <summary>
	/// Returns wether or not this given key is pressed down in this frame. 
	/// </summary>
	public bool GetKeyDown(string Name){
		return MapEntries[Name].KeyDown;
	}
	
	/// <summary>
	/// Returns wether or not this given key is released in this frame. 
	/// </summary>
	public bool GetKeyUp(string Name){
		return MapEntries[Name].KeyUp;
	}
	
	/// <summary>
	/// Returns wether or not this given key is currently pressed. 
	/// </summary>
	public bool GetKey(string Name){
		return MapEntries[Name].KeyHeld;
	}
	
	/// <summary>
	/// Fills the AxesStrings with known axes. these should match the ones in the project's input settings.  
	/// </summary>
	private void InitAxes(){
		AxesPrevValues = new float[NumberOfJoysticks+1,NumberOfAxes+1];
		AxesStrings = new string[NumberOfJoysticks+1,NumberOfAxes+1];
		
		for (int i = 1; i <= NumberOfJoysticks; i++) {
			for (int j = 1; j <= NumberOfAxes; j++) {
				AxesStrings[i,j] = "Joy" + i + " Axis " + j;
			}
		}
	}
	
	/// <summary>
	/// Sets current axis values so that they can be checked on wether or not they changed during key scanning.
	/// </summary>
	private void InitCheckAxes(){
		for (int i = 1; i <= NumberOfJoysticks; i++) {
			for (int j = 1; j <= NumberOfAxes; j++) {
				AxesPrevValues[i,j] = Input.GetAxisRaw(AxesStrings[i,j]);
			}
		}
	}
	
	/// <summary>
	/// Starts the scanning.
	/// </summary>
	public void StartScanning(){
		InitCheckAxes(); //Neded so that the scan only checks axes if they change, usefull for throttle axes that don't automatically default to 0.
		Scanning = true;
	}
	
	/// <summary>
	/// Checks the entry input. Needs to be done every update. 
	/// </summary>
	private void CheckEntryInput(InputMapEntry MapEntry){
		bool PrimaryDown; 
		bool PrimaryUp; 
		bool PrimaryHeld;
		float PrimaryValue = 0;
		bool SecondaryDown = false; 
		bool SecondaryUp = false; 
		bool SecondaryHeld = false;
		float SecondaryValue = 0;
		if (MapEntry.PrimaryIsAxis){
			CheckAxis(MapEntry.PrimaryAxisName, MapEntry.PrimaryModifier, out PrimaryDown, out PrimaryUp, out PrimaryHeld, out PrimaryValue, MapEntry.PrimaryAxisDeadzone, MapEntry.PrimaryAxisValue);
		}else{
			CheckKey(MapEntry.PrimaryKey, MapEntry.PrimaryModifier, out PrimaryDown, out PrimaryUp, out PrimaryHeld);
		}

		if (MapEntry.SecondaryIsAxis){
			CheckAxis(MapEntry.SecondaryAxisName, MapEntry.SecondaryModifier, out SecondaryDown, out SecondaryUp, out SecondaryHeld, out SecondaryValue, MapEntry.SecondaryAxisDeadzone, MapEntry.SecondaryAxisValue);
		}else{
			CheckKey(MapEntry.SecondaryKey, MapEntry.SecondaryModifier, out SecondaryDown, out SecondaryUp, out SecondaryHeld);
		}
		
		MapEntry.KeyUp = PrimaryUp || SecondaryUp;
		//Failsafe for KeyUp checking. Should theoretically fix things like Alt-Tabbing not registering Alt's release event.
		if (MapEntry.KeyHeld && !PrimaryHeld && !SecondaryHeld){
			MapEntry.KeyUp = true;
		}
		MapEntry.KeyDown = PrimaryDown || SecondaryDown;
		MapEntry.KeyHeld = PrimaryHeld || SecondaryHeld;
		MapEntry.PrimaryAxisValue = PrimaryValue;
		MapEntry.SecondaryAxisValue = SecondaryValue;
	}
	
	/// <summary>
	/// Checks the current state of the modifier.
	/// </summary>
	private void CheckModifier(KeyCode Modifier){
		Modifiers[Modifier] = Input.GetKeyDown(Modifier)? 1 : Input.GetKey(Modifier) ? 2 : Input.GetKeyUp(Modifier)? 4 : 0;
		if (Modifiers[Modifier] == 1 || Modifiers[Modifier] == 2){
			ModifierPressed = true;
		}
	}
	
	/// <summary>
	/// Checks the state of the mapped key.
	/// </summary>
	private void CheckKey(KeyCode Key, KeyCode Modifier, out bool Down, out bool Up, out bool Held){
		int KeyVal = Input.GetKeyDown(Key)? 8 : Input.GetKey(Key) ? 16 : Input.GetKeyUp(Key)? 32 : 0;
		int ModVal = Modifier == KeyCode.None ? 0 : Modifiers[Modifier];
		
		//If a modifier is pressed, and the modifier for this key is either none or not pressed, return.
		//This is to prevent keys with multiple modifiers to register each of the mapped input.
		if (ModifierPressed && (Modifier == KeyCode.None || ModVal != 0)){
			if (ModifiedKeys.Contains(Key)){
				Down = false;
				Held = false;
				Up = false;
				return;
			}
		}
		
		//If a modifier is not pressed, but there is a modifier set for this key, return. 
		//This is to prevent keys with multiple modifiers to register each of the mapped input.
		if (!ModifierPressed && !(Modifier == KeyCode.None)){
			Down = false;
			Held = false;
			Up = false;
			return;
		}
		
		//Check the current states of the key and modifier to see what return states should be
		int total = ModVal + KeyVal;
		Down = (total == 8 || total == 9 || total == 10 || total == 17);
		Up = (total == 32 || total == 36 || total == 20);
		Held = (total == 16 || total == 18 || Down);
	}
	
	/// <summary>
	/// Checks the state of the mapped axis.
	/// </summary>
	private void CheckAxis(string AxisName, KeyCode Modifier, out bool Down, out bool Up, out bool Held, out float AxisValue, float Deadzone, float PrevValue){
		int KeyVal = 0;
		int ModVal = Modifier == KeyCode.None ? 0 : Modifiers[Modifier];
		AxisValue = 0;
		
		//If a modifier is pressed, and the modifier for this axis is either none or not pressed, return.
		//This is to prevent axes with multiple modifiers to register each of the mapped input.
		if (ModifierPressed && (Modifier == KeyCode.None || ModVal != 0)){
			if (ModifiedAxes.Contains(AxisName)){
				Down = false;
				Held = false;
				Up = false;
				AxisValue = 0;
				return;
			}
		}
		
		//If a modifier is not pressed, but there is a modifier set for this axis, return. 
		//This is to prevent axes with multiple modifiers to register each of the mapped input.
		if (!ModifierPressed && !(Modifier == KeyCode.None)){
			Down = false;
			Held = false;
			Up = false;
			AxisValue = 0;
			return;
		}
		
		
		float tempAxisValue = Input.GetAxisRaw(AxisName.Remove(AxisName.Length-1, 1));
		if ((AxisName.EndsWith("+") && tempAxisValue > 0) || AxisName.EndsWith("-") && tempAxisValue < 0){
			AxisValue = Mathf.Abs(tempAxisValue);
			tempAxisValue = Mathf.Abs(tempAxisValue);
			if (tempAxisValue >= Deadzone){
				if (Mathf.Abs(PrevValue) >= Deadzone){
					KeyVal = 16;
				}else{
					KeyVal = 8;
				}
			}else{
				if (Mathf.Abs(PrevValue) >= Deadzone){
					KeyVal = 32;
				}else{
					KeyVal = 0;
				}
			}
		}
		
		//Check the current states of the key and modifier to see what return states should be
		int total = ModVal + KeyVal;
		Down = (total == 8 || total == 9 || total == 10 || total == 17);
		Up = (total == 32 || total == 36 || total == 20);
		Held = (total == 16 || total == 18 || Down);
		
	}
	
	/// <summary>
	/// Sets the key.
	/// </summary>
	public void SetKey(string Name, KeyCode PrimaryKey, KeyCode SecondaryKey, KeyCode PrimaryModifier, KeyCode SecondaryModifier){
		if (!MapEntries.ContainsKey(Name)){
			MapEntries[Name] = new InputMapEntry();
		}
		MapEntries[Name].Name = Name;
		MapEntries[Name].PrimaryKey = PrimaryKey;
		MapEntries[Name].SecondaryKey = SecondaryKey;
		MapEntries[Name].PrimaryModifier = PrimaryModifier;
		MapEntries[Name].SecondaryModifier = SecondaryModifier;
		MapEntries[Name].PrimaryIsAxis = false;
		MapEntries[Name].SecondaryIsAxis = false;
		if (PrimaryModifier != KeyCode.None){
			SetModifier(PrimaryKey, PrimaryModifier);
		}
		if (SecondaryModifier != KeyCode.None){
			SetModifier(SecondaryKey, SecondaryModifier);
		}
	}
	
	/// <summary>
	/// Sets the modifier. This is neded for easier checking of modifiers 
	/// and to prevent modified keys from registering unmodified key events when a modifier is pressed
	/// </summary>
	private void SetModifier(KeyCode Key, KeyCode Modifier){
		if (!ModifiedKeys.Contains(Key)){
			ModifiedKeys.Add(Key);
		}
		if (!Modifiers.ContainsKey(Modifier)){
			Modifiers[Modifier] = 0;
			ModifierKeys = new List<KeyCode>(Modifiers.Keys);
		}
	}
	
	/// <summary>
	/// Sets the modifier for axes. 
	/// </summary>
	private void SetModifier(string Axis, KeyCode Modifier){
		if (!ModifiedAxes.Contains(Axis)){
			ModifiedAxes.Add(Axis);
		}
		if (!Modifiers.ContainsKey(Modifier)){
			Modifiers[Modifier] = 0;
			ModifierKeys = new List<KeyCode>(Modifiers.Keys);
		}
	}
	
	/// <summary>
	/// Sets a modifier only. This can be used to allow scanning on modifiers that aren't set in the defaults.
	/// </summary>
	public void SetModifier(KeyCode Modifier){
		if (!Modifiers.ContainsKey(Modifier)){
			Modifiers[Modifier] = 0;
			ModifierKeys = new List<KeyCode>(Modifiers.Keys);
		}
	}
	
	/// <summary>
	/// Sets the primary key.
	/// </summary>
	public void SetPrimaryKey(string Name, KeyCode PrimaryKey, KeyCode PrimaryModifier){
		if (!MapEntries.ContainsKey(Name)){
			MapEntries[Name] = new InputMapEntry();
		}
		InputMapEntry Entry = MapEntries[Name];
		Entry.Name = Name;
		Entry.PrimaryKey = PrimaryKey;
		Entry.PrimaryModifier = PrimaryModifier;
		MapEntries[Name].PrimaryIsAxis = false;
		if (PrimaryModifier != KeyCode.None){
			SetModifier(PrimaryKey, PrimaryModifier);
		}
	}
	
	/// <summary>
	/// Sets the secondary key.
	/// </summary>
	public void SetSecondaryKey(string Name, KeyCode SecondaryKey, KeyCode SecondaryModifier){
		if (!MapEntries.ContainsKey(Name)){
			MapEntries[Name] = new InputMapEntry();
		}
		InputMapEntry Entry = MapEntries[Name];
		Entry.Name = Name;
		Entry.SecondaryKey = SecondaryKey;
		Entry.SecondaryModifier = SecondaryModifier;
		MapEntries[Name].SecondaryIsAxis = false;
		if (SecondaryModifier != KeyCode.None){
			SetModifier(SecondaryKey, SecondaryModifier);
		}
	}
	
	/// <summary>
	/// Sets the primary key as axis.
	/// </summary>
	public void SetPrimaryKeyAsAxis(string Name, string PrimaryAxisName, KeyCode PrimaryModifier, float PrimaryAxisDeadzone){
		if (!MapEntries.ContainsKey(Name)){
			MapEntries[Name] = new InputMapEntry();
		}
		InputMapEntry Entry = MapEntries[Name];
		Entry.Name = Name;
		Entry.PrimaryAxisName = PrimaryAxisName;
		Entry.PrimaryModifier = PrimaryModifier;
		Entry.PrimaryAxisDeadzone = PrimaryAxisDeadzone;
		Entry.PrimaryIsAxis = true;
		
		if (PrimaryModifier != KeyCode.None){
			SetModifier(PrimaryAxisName, PrimaryModifier);
		}
	}
	
	/// <summary>
	/// Sets the secondary key as axis.
	/// </summary>
	public void SetSecondaryKeyAsAxis(string Name, string SecondaryAxisName, KeyCode SecondaryModifier, float SecondaryAxisDeadzone){
		if (!MapEntries.ContainsKey(Name)){
			MapEntries[Name] = new InputMapEntry();
		}
		InputMapEntry Entry = MapEntries[Name];
		Entry.Name = Name;
		Entry.SecondaryAxisName = SecondaryAxisName;
		Entry.SecondaryModifier = SecondaryModifier;
		Entry.SecondaryAxisDeadzone = SecondaryAxisDeadzone;
		Entry.SecondaryIsAxis = true;
		
		if (SecondaryModifier != KeyCode.None){
			SetModifier(SecondaryAxisName, SecondaryModifier);
		}
	}
	
	/// <summary>
	/// Gets the axis.
	/// </summary>
	public float GetAxis(string Name){
		float AxisVal = 0; 
		
		AxisVal = CheckAxisKey(MapEntries[AxisEntries[Name].KeyPositive]);
		if (AxisVal == 0){
			AxisVal = -CheckAxisKey(MapEntries[AxisEntries[Name].KeyNegative]);
		}
		return AxisVal;	
	}
	
	/// <summary>
	/// Checks the axis key.
	/// </summary>
	private float CheckAxisKey(InputMapEntry Entry){
		float AxisVal = 0; 
		if (Entry.PrimaryIsAxis){
			AxisVal += Entry.PrimaryAxisValue;
		}else if (Entry.SecondaryIsAxis){
			AxisVal += Entry.SecondaryAxisValue;
		}
		if (AxisVal == 0){
			if (Entry.KeyHeld){
				AxisVal += 1;
			}
		}
		return AxisVal;
	}
			
	/// <summary>
	/// Sets the axis.
	/// </summary>
	public void SetAxis(string Name, string KeyPositive, string KeyNegative){
		if (!AxisEntries.ContainsKey(Name)){
			AxisEntries[Name] = new InputAxisEntry();
		}
		InputAxisEntry Entry = AxisEntries[Name];
		Entry.Name = Name;
		Entry.KeyPositive = KeyPositive;
		Entry.KeyNegative = KeyNegative;
	}
	
	/// <summary>
	/// Scans the input. 
	/// </summary>
	private void ScanInput(){
		KeyCode modifierKey = KeyCode.None;
		if (Input.anyKey){
			//Check if any modifier keys are pressed. 
			foreach (KeyCode key in ModifierKeys){
				if (Input.GetKey(key)){
					modifierKey = key;
					break; //Only one modifier is possible
				}
			}
			
			//Check if any keys are pressed. Throws event if a key is pressed that is not a modifier.
			for (int i = (int)KeyCode.None; i < NrOfKeyCodes; i++) {
				KeyCode key = (KeyCode)Enum.GetValues(typeof(KeyCode)).GetValue(i);
				if (Input.GetKeyDown(key) && !ModifierKeys.Contains(key)){
					if (OnKeyScanned != null){
						OnKeyScanned(key, modifierKey);
					}	
					Scanning = false;
					return; //We only need to have one key or axis scanned.
				}
			}
		}
		
		//Check for joystick axes. Only register when an axis value actually changes. 
		//This prevents axes that continuously give input (like throttles) to mess up key scanning
		for (int i = 1; i <= NumberOfJoysticks; i++) {
			for (int j = 1; j <= NumberOfAxes; j++) {
				float AxisValue = Input.GetAxisRaw(AxesStrings[i,j]);
				if (AxesPrevValues[i,j] != AxisValue){
					string suffix = "";
					if (AxisValue > 0){
						suffix = "+";
					}else if (AxisValue < 0){
						suffix = "-";
					}
					
					if (OnAxisScanned != null){
						OnAxisScanned(AxesStrings[i,j]+suffix, modifierKey);
					}
					Scanning = false; 
					return; //We only need to have one key or axis scanned.
				}
			}
		}
		
		//This allows modifier keys to be used as normal keys aswell. 
		foreach (KeyCode key in ModifierKeys){
			if (Input.GetKeyUp(key)){
				if (OnKeyScanned != null){
					OnKeyScanned(key, KeyCode.None);
				}
				Scanning = false;
				return;
			}
		}
	}
	
	/// <summary>
	/// Saves the mapping to an XML text. Does not actually create a file. 
	/// </summary>
	public string Save(){
		StringBuilder builder = new StringBuilder();
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
		{
			Indent = true,
			IndentChars = "\t",
			NewLineOnAttributes = true
		};
		
		using (XmlWriter writer = XmlWriter.Create(builder, xmlWriterSettings)){
			writer.WriteStartDocument();
			writer.WriteStartElement("Input");
			writer.WriteStartElement("Keys");
			foreach(InputMapEntry MapEntry in MapEntries.Values){
				writer.WriteStartElement("Key");
				writer.WriteAttributeString("Name", MapEntry.Name);
				writer.WriteAttributeString("PrimaryKey", MapEntry.PrimaryKey.ToString());
				writer.WriteAttributeString("SecondaryKey", MapEntry.SecondaryKey.ToString());
				writer.WriteAttributeString("PrimaryModifier", MapEntry.PrimaryModifier.ToString());
				writer.WriteAttributeString("SecondaryModifier", MapEntry.SecondaryModifier.ToString());
				writer.WriteAttributeString("PrimaryIsAxis", MapEntry.PrimaryIsAxis.ToString());
				writer.WriteAttributeString("PrimaryAxisName", MapEntry.PrimaryAxisName);
				writer.WriteAttributeString("PrimaryAxisDeadzone", MapEntry.PrimaryAxisDeadzone.ToString());
				writer.WriteAttributeString("SecondaryIsAxis", MapEntry.SecondaryIsAxis.ToString());
				writer.WriteAttributeString("SecondaryAxisName", MapEntry.SecondaryAxisName);
				writer.WriteAttributeString("SecondaryAxisDeadzone", MapEntry.SecondaryAxisDeadzone.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("Axes");
			foreach(InputAxisEntry MapEntry in AxisEntries.Values){
				writer.WriteStartElement("Axis");
				writer.WriteAttributeString("Name", MapEntry.Name);
				writer.WriteAttributeString("KeyPositive", MapEntry.KeyPositive);
				writer.WriteAttributeString("KeyNegative", MapEntry.KeyNegative);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}	
		return builder.ToString();
	}
	
	/// <summary>
	/// Load the specified FileContents.
	/// </summary>
	/// ModifyExistingEntriesOnly : When true, Only modify keys that allready exist. 
	/// This is used to prevent old input config files from adding back deprecated keys.
	public void Load(string FileContents, bool ModifyExistingEntriesOnly){
		using (XmlReader reader = XmlReader.Create(new StringReader(FileContents))){
			while (reader.Read()){
				if (reader.IsStartElement()){
					switch (reader.Name){
						case "Key":
							InputMapEntry ReadEntry = new InputMapEntry();
							ReadEntry.Name = reader.GetAttribute("Name");
							ReadEntry.PrimaryKey = (KeyCode)Enum.Parse(typeof(KeyCode), reader.GetAttribute("PrimaryKey"));
							ReadEntry.SecondaryKey = (KeyCode)Enum.Parse(typeof(KeyCode),reader.GetAttribute("SecondaryKey"));
							ReadEntry.PrimaryModifier = (KeyCode)Enum.Parse(typeof(KeyCode),reader.GetAttribute("PrimaryModifier"));
							ReadEntry.SecondaryModifier = (KeyCode)Enum.Parse(typeof(KeyCode),reader.GetAttribute("SecondaryModifier"));
							bool.TryParse(reader.GetAttribute("PrimaryIsAxis"), out ReadEntry.PrimaryIsAxis);
							ReadEntry.PrimaryAxisName = reader.GetAttribute("PrimaryAxisName");
							float.TryParse(reader.GetAttribute("PrimaryAxisDeadzone"), out ReadEntry.PrimaryAxisDeadzone);
							bool.TryParse(reader.GetAttribute("SecondaryIsAxis"), out ReadEntry.SecondaryIsAxis);
							ReadEntry.SecondaryAxisName = reader.GetAttribute("SecondaryAxisName");
							float.TryParse(reader.GetAttribute("SecondaryAxisDeadzone"), out ReadEntry.SecondaryAxisDeadzone);
						
							if (ModifyExistingEntriesOnly){
								if (MapEntries.ContainsKey(ReadEntry.Name)){
									MapEntries[ReadEntry.Name] = ReadEntry;
									//Make sure keys are properly set as modifiers 
									if (ReadEntry.PrimaryModifier != KeyCode.None){
										if (ReadEntry.PrimaryKey != KeyCode.None){
											SetModifier(ReadEntry.PrimaryKey, ReadEntry.PrimaryModifier);
										}
										if (ReadEntry.PrimaryAxisName != ""){
											SetModifier(ReadEntry.PrimaryAxisName, ReadEntry.PrimaryModifier);
										}
									}
									if (ReadEntry.SecondaryModifier != KeyCode.None){
										if (ReadEntry.SecondaryKey != KeyCode.None){
											SetModifier(ReadEntry.SecondaryKey, ReadEntry.SecondaryModifier);
										}
										if (ReadEntry.SecondaryAxisName != ""){
											SetModifier(ReadEntry.SecondaryAxisName, ReadEntry.SecondaryModifier);
										}
									}
								}
							}else{
								MapEntries[ReadEntry.Name] = ReadEntry;
								//Make sure keys are properly set as modifiers 
								if (ReadEntry.PrimaryModifier != KeyCode.None){
									if (ReadEntry.PrimaryKey != KeyCode.None){
										SetModifier(ReadEntry.PrimaryKey, ReadEntry.PrimaryModifier);
									}
									if (ReadEntry.PrimaryAxisName != ""){
										SetModifier(ReadEntry.PrimaryAxisName, ReadEntry.PrimaryModifier);
									}
								}
								if (ReadEntry.SecondaryModifier != KeyCode.None){
									if (ReadEntry.SecondaryKey != KeyCode.None){
										SetModifier(ReadEntry.SecondaryKey, ReadEntry.SecondaryModifier);
									}
									if (ReadEntry.SecondaryAxisName != ""){
										SetModifier(ReadEntry.SecondaryAxisName, ReadEntry.SecondaryModifier);
									}
								}
							}
						break;
						case "Axis":
							InputAxisEntry ReadAxis = new InputAxisEntry();
							ReadAxis.Name = reader.GetAttribute("Name");
							ReadAxis.KeyPositive = reader.GetAttribute("KeyPositive");
							ReadAxis.KeyNegative = reader.GetAttribute("KeyNegative");
							if (ModifyExistingEntriesOnly){
								if (AxisEntries.ContainsKey(ReadAxis.Name)){
									AxisEntries[ReadAxis.Name] = ReadAxis;
								}
							}else{
								AxisEntries[ReadAxis.Name] = ReadAxis;
							}
						break;
					}
				}
			}
		}
	}
}
