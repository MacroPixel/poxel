using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;

// Does JSON parsing without having to specify type beforehand
// I'm sure something like this exists, but I couldn't find one
// that easily, so I decided it'd be easier to make it myself
namespace sjson
{
    // There are 7 types that are accepted
    public enum Type
    {
        Null,
        Int,
        Float,
        String,
        Bool,
        List,
        Object
    }

    // Every type derives from this
    // Polymorphism is used so I can arbitrarily cast to whichever type I need to later on
    public abstract class Element
    {
        public Type type;

        // Check whether an element can be downcast to a type
        public bool is_type< T >() where T: class => ( ( this as T ) != null );

        // Return the value of it after downcasting
        public C to_type< C >() where C: class => ( this as C );
    }

    // All of the following 6 classes derive from element and
    // will be accessed via downcasting later on

    public class Null : Element
    {
        public Null()
        {
            type = Type.Null;
        }
    }

    public class Int : Element
    {
        public int val;

        public Int( int initial_val = 0 )
        {
            type = Type.Int;
            val = initial_val;
        }

        public static implicit operator int( Int o ) => o.val;
    }

    public class Float : Element
    {
        public float val;

        public Float( float initial_val = 0.0f )
        {
            type = Type.Float;
            val = initial_val;
        }

        public static implicit operator float( Float o ) => o.val;
    }

    public class String : Element
    {
        public string val;

        public String( string initial_val = "" )
        {
            type = Type.String;
            val = initial_val;
        }

        public static implicit operator string( String o ) => o.val;
    }

    public class Bool : Element
    {
        public bool val;

        public Bool( bool initial_val = false )
        {
            type = Type.Bool;
            val = initial_val;
        }

        public static implicit operator bool( Bool o ) => o.val;
    }

    public class List : Element
    {
        public List<Element> val;

        public List()
        {
            type = Type.List;
            val = new List<Element>();
        }

        public static implicit operator List<Element>( List o ) => o.val;
    }

    public class Object : Element
    {
        public Dictionary<string, Element> val;

        public Object()
        {
            type = Type.Object;
            val = new Dictionary<string, Element>();
        }

        public static implicit operator Dictionary<string,Element>( Object o ) => o.val;
    }

    // For error handling
    public class JSON_Exception : Exception
    {
        public JSON_Exception() { }

        public JSON_Exception( string message )
            : base( message ) { }

        public JSON_Exception( string message, Exception inner )
            : base( message, inner ) { }
    }

    // A namespace for functions since apparently you
    // can't put functions in a namespace in C# (???)
    public class op
    {
        static readonly string n_first_chars;
        static readonly string n_chars;

        static op()
        {
            n_first_chars = "-0123456789.";
            n_chars = "-0123456789.eE";
        }

        // Parses the file into a JSON object
        // Calls a function that parses JSON from a string
        public static Element parse_file( string file_path )
        {
            string file = string.Join( "", File.ReadAllLines( file_path ) );
            return parse( file );
        }

        // Where the magic happens
        // Adapted from https://gist.github.com/ptrelford/e98ccda0aa4b406dea4f37e3c5a75ae9
        public static Element parse( string input_string )
        {
            // `i` is the current index of the string that's being read
            // It starts at the first non-whitespace position
            int i = pos_after_white_space( 0, input_string );
            if ( i == input_string.Length )
                throw new JSON_Exception( "Empty file" );

            return parse_value( ref i, input_string );
        }

        public static Element parse_value( ref int i, string str )
        {
            // `c` is the current character that index `i` maps to
            char c = str[i];

            // Return whatever object this initially parses to
            if ( c == '{' )
                return parse_object( ref i, str );
            else if ( c == '[' )
                return parse_list( ref i, str );
            else if ( c == '\"' || c == '\'' )
                return parse_string( ref i, str );
            else if ( n_first_chars.Contains( char.ToString( c ) ) )
                return parse_number( ref i, str );
            else if ( c == 't' )
                return parse_literal( ref i, str, "true", new Bool( true ) );
            else if ( c == 'f' )
                return parse_literal( ref i, str, "false", new Bool( false ) );
            else if ( c == 'n' )
                return parse_literal( ref i, str, "null", new Null() );
            else
            {
                // string substr_1 = str.Substring( Math.Max( i - 3, 0 ), Math.Min( str.Length - j, 9 ) );
                throw new JSON_Exception( "Invalid JSON" );
            }
        }

        // Helper functions for above function
        private static bool is_white_space( char c )
        {
            return c == ' ' || c == '\r' || c == '\n' || c == '\t';
        }

        private static int pos_after_white_space( int i, string str )
        {
            while ( i < str.Length && is_white_space( str[i] ) )
            {
                i ++;
            }
            return i;
        }

        // Compares a string against string_match, returning `output_value` if it matches
        private static Element parse_literal( ref int i, string str, string string_match, Element output_value )
        {
            // Must match length and value
            if ( string_match.Length > str.Length - i )
                throw new JSON_Exception( "Expecting " + string_match );
            for ( int ii = 0; ii < string_match.Length; ii ++ )
                if ( string_match[ii] != str[i++] )
                    throw new JSON_Exception( "Expecting " + string_match );
            return output_value;
        }

        private static Element parse_number( ref int i, string str )
        {
            // Keep reading characters until non-numeric character is encountered
            string output = "";
            while ( i < str.Length && n_chars.Contains( char.ToString( str[i] ) ) )
            {
                output += str[i];
                i ++;
            }

            try
            {
                // Convert to float if contains decimals or scientific notation
                if ( output.Contains( "." ) || output.Contains( "e" ) || output.Contains( "E" ) )
                    return new Float( float.Parse( output ) );

                // Otherwise, convert to int
                else
                    return new Int( int.Parse( output ) );
            }
            catch ( System.FormatException )
            {
                throw new JSON_Exception( string.Format( "Invalid number \"{0}\"", output ) );
            }
        }

        private static Element parse_string( ref int i, string str )
        {
            // Keep reading characters until non-escaped " or ' is encountered
            string output = "";
            char quote = str[i]; // The type of quote this opened with
            i ++; // Ignore opening quote

            while ( str[i] != quote )
            {
                // Escape sequences increment `i` again before storing its value in output
                if ( str[i] == '\\' )
                {
                    i ++;
                    if ( str[i] == 'n' )
                        output += '\n';
                    else if ( str[i] == 't' )
                        output += '\t';
                    else if ( str[i] == 'r' )
                        output += '\r';
                    else if ( str[i] == 'f' )
                        output += '\f';
                    else
                        output += str[i];
                }
                // Non-escape sequences are added to the output as-is
                // They've already been checked against `quote` in the while condition
                else
                    output += str[i];
                i ++;
            }

            // Increment `i` once more to skip over the quote
            // (this works because `i` is passed by reference)
            i ++;

            return new String( output );
        }

        private static Element parse_object( ref int i, string str )
        {
            // Create object
            i = pos_after_white_space( i + 1, str ); // Ignore opening brace & whitespace
            Object output = new Object();

            // Iterate through the whole object
            while ( true )
            {
                // Close object if there's a closing brace
                // This will be fired if the object is empty ("{}") or
                // if there's a trailing comma
                if ( str[i] == '}' )
                {
                    i ++;
                    return output;
                }

                // Read the current key
                // The key must be a string for the time being
                Element key_element = parse_value( ref i, str );
                String key_string = ( key_element as String );
                string key;
                if ( key_string != null )
                    key = key_string.val;
                else
                    throw new JSON_Exception( "Keys must be strings" );

                // Read the colon between key:value
                i = pos_after_white_space( i, str );
                if ( str[i] != ':' )
                    throw new JSON_Exception( "Expecting : between key and value" );

                // Read the current value
                i = pos_after_white_space( i + 1, str );
                Element val = parse_value( ref i, str );
                output.val[ key ] = val;

                // Continue to next key if there's a comma
                if ( str[i] == ',' )
                    i = pos_after_white_space( i + 1, str );
                // Break if there's no comma
                // (either because of an error or because it's the end of the object)
                else
                    break;
            }

            // If we end up here, it better be the end of the object
            i = pos_after_white_space( i, str );
            if ( str[i] != '}' )
                throw new JSON_Exception( "Expecting }" );

            // Increment past the closing brace and return the object
            i ++;
            return output;
        }

        private static Element parse_list( ref int i, string str )
        {
            // Create list
            i = pos_after_white_space( i + 1, str ); // Ignore opening bracket & whitespace
            List output = new List();

            // Iterate through the whole list
            while ( true )
            {
                // Close list if there's a closing brace
                if ( str[i] == ']' )
                {
                    i ++;
                    return output;
                }

                // Read the current value
                Element val = parse_value( ref i, str );
                output.val.Add( val );

                // Continue to next value if there's a comma
                if ( str[i] == ',' )
                    i = pos_after_white_space( i + 1, str );
                // Break if there's no comma
                else
                    break;
            }

            // If we end up here, it better be the end of the list
            i = pos_after_white_space( i, str );
            if ( str[i] != ']' )
                throw new JSON_Exception( "Expecting ]" );

            // Increment past the closing bracket and return the list
            i ++;
            return output;
        }
    }
}