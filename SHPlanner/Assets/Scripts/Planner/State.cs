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

  public class WorldState
  {
    public class Symbol
    {
      public enum Types { Integer, Float, Boolean, Vector3 }
      public struct Union
      {
        public int Integer;
        public bool Boolean;
        public float Float;
        public Vector3 Vector3;
      }

      public string Name;
      public Union Value = new Union();
      public Transform Subject;
      Types Type;

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
        Type = Types.Float;
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

    /// <summary>
    /// Adds a symbol to this world state.
    /// </summary>
    /// <param name="symbol"></param>
    public void Add(Symbol symbol)
    {
      Symbols.Add(symbol.Name, symbol);
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
            Trace.Script(Symbols[symbol.Key].Print() + " is not equal to " + symbol.Value.Print());
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
    /// Merges the symbols of the other world state with this one. 
    /// It will overwrite matching symbols, and add any not found in the current one.
    /// </summary>
    /// <param name="other"></param>
    public void Merge(WorldState other)
    {
      foreach (var symbol in other.Symbols)
      {
        // Overwrite matching symbols
        if (Symbols.ContainsKey(symbol.Key))
        {
          Symbols[symbol.Key] = symbol.Value;
        }
        // Add any ones not present
        else
        {
          Add(symbol.Value);
        }
      }
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