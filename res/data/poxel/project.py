from poxel.element import *

# Should encompass all information relating to the current project
# Ran in the main file using `.run()` along with `sys.argv`
class Project:

    def __init__( self ):
        
        # Represents everything present in the project
        # Should only hold `Element` class and its derivatives
        self._contents = []

    # Adds an element to the timeline
    # No protection for adding elements more than once
    def add_element( self, element ):

        # Must be an Element or a child of Element
        if not isinstance( element, Element ):
            raise TypeError( 'Non-Element class' )

        self._contents.append( element )

    # Removes elements matching a condition from the timeline
    # There is almost never a good reason to use this function
    def remove_element( self, condition_func ):

        # Remove elements matching lambda
        self._contents = [ e for e in self.contents if condition_func( e ) ]

    # Takes in command arguments and spits information out into the console
    # This information can then be read within Unity through a subprocess
    def run( self, command_args ):

        # If the script is run without any arguments
        if len( command_args ) == 1:
            print( "If you're seeing this, then you ran this Python file directly without any arguments." )
            print( "For proper functionality, link this file to a Poxel project." )

        # SYNTAX: [script_name] [frame (int)]
        # Compute an editor state based off of the frame input
        elif ( len( command_args ) == 2 ):
            
            # Attempt to cast from arguments
            try:
                f = int( command_args[1] )
            except TypeError:
                raise TypeError( 'Arguments for retrieving editor state must be numeric (int, float)' )

            print( '' )

        # Erroneous arguments
        else:
            raise RuntimeError( 'Invalid arguments.' )

    @property
    def contents( self ):
        return self._contents.copy();