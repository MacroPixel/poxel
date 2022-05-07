from poxel import utils
from poxel.frame import *

class Element:

    # Controls sequential IDs
    _current_id = 0

    # Maps a class to a string representation when converting between Unity and Python
    _class_strings = {}

    def __init__( self, start, end ):

        # Elements are assigned an ID upon creation
        # This is necessary so Unity can keep track of objects between frames
        self._id = self.new_id()

        # Elements must have a frame at which they start and end
        # This also must be a positive integer
        self._start = utils.assert_type( start, Frame )
        self._end = utils.assert_type( end, Frame )

        # Element instances are stored as JSON objects within Unity
        # Any variables that aren't within this dictionary aren't passed into said object
        # (Exluding basic variables like `start` and `end`)
        # Also, any objects present must return a JSON-parseable string within __repr__()
        self.data = {}

    # Creates a JSON object that can be sent to Unity
    def to_json( self ):

        # Class string should be defined within <TODO>
        return {
            "class": self.class_to_string( self.__class__ ),
            "id": self.id,
            "bounds": [ self.start, self.end ],
            "data": self.data
        }

    # Converts object to string representation
    # Children are free, but not recommended, to overwrite this
    def __repr__( self ):

        return repr( self.to_json() )

    # Returns all data about a given element at a specific time index
    # At its base, Elements don't actually return anything
    # They should be derived from to return things like images, video,
    # composites, etc.
    def get_state_at( self, frame ):

        utils.assert_type( frame, Frame )

    # Returns the next sequential ID
    # Works in derived classes as well so long as they don't overwrite the function
    @staticmethod
    def new_id():

        Element._current_id += 1
        return Element._current_id

    # Allows a class to be converted to string and vice versa
    # !! DO NOT OVERWRITE !!
    @staticmethod
    def add_class_string( class_string, class_type ):

        # Must be (Element, string)
        if ( not issubclass( class_type, Element ) ):
            raise TypeError( f'{ class_type } must be derived from Element' )
        utils.assert_type( class_string, str )

        # No duplicate entires (either in keys or values)
        if ( class_type in Element._class_strings.keys() ):
            raise RuntimeError( f'Class { class_type } is already associated with a string.' )
        if ( class_string in Element._class_strings.values() ):
            raise RuntimeError( f'String { class_string } is already associated with a class' )

        Element._class_strings[ class_string ] = class_type

    # Switch between class type and class string
    @staticmethod
    def string_to_class( class_string ):

        try:
            return Element._class_strings[ class_string ]
        except KeyError:
            return None

    @staticmethod
    def class_to_string( class_type ):

        try:
            return [ s for s in Element._class_strings if Element._class_strings[ s ] == class_type ][0]
        except IndexError:
            return None

    @property
    def id( self ):
        return self._id

    @property
    def start( self ):
        return self._start

    @property
    def end( self ):
        return self._end

# Define element as class
Element.add_class_string( "Element", Element )