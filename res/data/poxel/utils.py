
def assert_type( value, datatype ):

    if isinstance( value, datatype ):
        return value
    raise TypeError( f"'{ value }' is not type { datatype }." )