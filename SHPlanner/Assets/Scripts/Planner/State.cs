/******************************************************************************/
/*!
@file   WorldState.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prototype
{
  /// <summary>
  /// In order to search the space of actions, the planner needs to present
  /// the state of the world in some way that lets it easily apply the preconditions
  /// and effects of actions. One compact way to represent the state of the world
  /// is with a list of world property symbols that contain an enumarated attribute key,
  /// a value.
  /// </summary>
  public class WorldState
  {
    /// <summary>
    /// Represents a property in the world (that is relevant to the planner)
    /// </summary>
    public class Symbol
    {
      public enum Types { Integer, Float, Boolean, Vector3 }
      public struct Union
      {
        public int Integer;
        public bool Boolean;
        public float Float;
        public Vector3 Vector3;
        public bool Compare(Union other)
        {
          if (this.Integer != other.Integer) return false;
          if (this.Boolean != other.Boolean) return false;
          if (this.Float != other.Float) return false;
          if (this.Vector3 != other.Vector3) return false;
          return true;
        }
      }

      public string Name;
      public Union Value = new Union();
      public Transform Subject;
      public Types Type;

      public Symbol(string name, bool value)
      {
        Name = name;
        Value.Boolean = value;
        Type = Types.Boolean;
      }

      public Symbol(string name, int value)
      {
        Name = name;
        Value.Integer = value;
        Type = Types.Integer;
      }

      public Symbol(string name, float value)
      {
        Name = name;
        Value.Float = value;
        Type = Types.Float;
      }

      public Symbol(string name, Vector3 value)
      {
        Name = name;
        Value.Vector3 = value;
        Type = Types.Vector3;
      }

      public Symbol(string name, Types type, Union value)
      {
        Name = name;
        Type = type;
        Value = value;
      }

      public T GetValue<T>()
      {
        switch (Type)
        {
          case Types.Boolean:
            return (T)Convert.ChangeType(Value.Boolean, typeof(bool));
          case Types.Integer:
            return (T)Convert.ChangeType(Value.Integer, typeof(int));
          case Types.Float:
            return (T)Convert.ChangeType(Value.Float, typeof(float));
          case Types.Vector3:
            return (T)Convert.ChangeType(Value.Vector3, typeof(Vector3));
        }

        throw new Exception("No proper type set!");
      }

      /// <summary>
      /// Compares this symbol with another.
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool Compare(Symbol other)
      {
        if (this.Type != other.Type)
          throw new Exception("Mismatching symbols are being compared!");

        switch (other.Type)
        {
          case Types.Boolean:
            return this.Value.Boolean == other.Value.Boolean;
          case Types.Integer:
            return this.Value.Integer == other.Value.Integer;
          case Types.Float:
            return this.Value.Float == other.Value.Float;
          case Types.Vector3:
            return this.Value.Vector3 == other.Value.Vector3;
        }

        throw new Exception("Wrong symbol?");
      }

      /// <summary>
      /// Prints the value of this symbol.
      /// </summary>
      /// <returns></returns>
      public string Print()
      {
        var builder = new StringBuilder();
        builder.Append(this.Name + " = ");
        switch (Type)
        {
          case Types.Boolean:
            builder.Append(Value.Boolean.ToString());
            break;
          case Types.Integer:
            builder.Append(Value.Integer.ToString());
            break;
          case Types.Float:
            builder.Append(Value.Float.ToString());
            break;
          case Types.Vector3:
            builder.Append(Value.Vector3.ToString());
            break;
        }

        return builder.ToString();
      }
    }


    Dictionary<string, Symbol> Symbols = new Dictionary<string, Symbol>();
    public bool IsEmpty { get { return Symbols.Count == 0; } }

    /// <summary>
    /// Returns the symbol with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Symbol At(string name)
    {
      if (Symbols.ContainsKey(name))
        return Symbols[name];
      return null;
    }

    /// <summary>
    /// Applies a symbol to this world state.
    /// </summary>
    /// <param name="symbol"></param>
    public void Apply(Symbol symbol)
    {
      if (Symbols.ContainsKey(symbol.Name))
        Symbols[symbol.Name].Value = symbol.Value;
      else
      {
        Symbols.Add(symbol.Name, new Symbol(symbol.Name, symbol.Type, symbol.Value));
      }
        //Symbols.Add(symbol.Name, symbol);
    }

    /// <summary>
    /// Applies a change to the given symbol in this world state. 
    /// If not present, it will add it.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void Apply(string name, bool value)
    {
      if (Symbols.ContainsKey(name))
        Symbols[name].Value.Boolean = value;
      else
        Symbols.Add(name, new Symbol(name, value));
    }
    
    /// <summary>
    /// Checks if this world state fulfills of the other.
    /// Compare all symbols.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Satisfies(WorldState other)
    {
      foreach (var symbol in other.Symbols)
      {
        // Look for a matching symbol
        if (Symbols.ContainsKey(symbol.Key))
        {
          // If the symbols were not equal...
          if (!Symbols[symbol.Key].Compare(symbol.Value))
          {
            //Trace.Script(Symbols[symbol.Key].Print() + " is not equal to " + symbol.Value.Print());
            return false;
          }
        }
        // The symbol was not found
        else
        {
          return false;
        }

      }

      // All symbols were a match
      return true;
    }

    /// <summary>
    /// Checks whether this WorldState contains the given symbol with the same value
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public bool Contains(Symbol symbol)
    {
      // Look for a matching symbol
      if (Symbols.ContainsKey(symbol.Name))
      {
        if (Symbols[symbol.Name].Value.Compare(symbol.Value))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Merges the symbols of the other world state with this one. 
    /// It will overwrite matching symbols, and add any not found in the current one.
    /// </summary>
    /// <param name="other"></param>
    public void Merge(WorldState other)
    {
      foreach (var otherSymbol in other.Symbols)
      {
        Apply(otherSymbol.Value);

        // Overwrite matching symbols
        //if (Symbols.ContainsKey(otherSymbol.Key))
        //{
        //  Symbols[otherSymbol.Key].Value = otherSymbol.Value.Value;
        //}
        //// Add any ones not present
        //else
        //{
        //  Apply(otherSymbol.Value);
        //}
      }
    }

    public WorldState Copy()
    {
      var newState = new WorldState();
      newState = this;
      return newState;
    } 

    /// <summary>
    /// Prints all the symbols along with their values
    /// </summary>
    /// <returns></returns>
    public string Print()
    {
      var builder = new StringBuilder();
      foreach (var symbol in Symbols)
      {
        builder.AppendFormat(" - {0}", symbol.Value.Print());
      }
      return builder.ToString();
    }


  }






}