from poxel import utils

# Holds a positive integer
class Frame:

    def __init__( self, value ):
        self.value = value

    def __repr__( self ):
        return str( self.value )

    @property
    def value( self ):
        return self._value

    # Accept the value, making sure it's a positive integer
    @value.setter
    def value( self, new ):

        self._value = utils.assert_type( new, int )
        if ( self.value < 0 ):
            raise RuntimeError( 'Frame must be positive.' )