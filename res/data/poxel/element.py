class Element:

    _current_id = 0

    def __init__( self, start, end ):

        # Elements are assigned an ID upon creation
        # This is necessary so Unity can keep track of objects between frames
        self._id = self.new_id()

        # Elements must have a frame at which they start and end
        # This also must be a positive integer
        try:
            self._start = int( start )
            self._end = int( end )
        except ValueError:
            raise TypeError( '`start` and `end` must be integers.' )
        if ( self.start < 0 or self.end < 0 ):
            raise RuntimeError( '`start` and `end` must be positive.' )

    # Returns the next sequential ID
    # Works in derived classes as well so long as they don't
    # overwrite the function
    @staticmethod
    def new_id():

        Element._current_id += 1
        return Element._current_id

    @property
    def id( self ):
        return self._id