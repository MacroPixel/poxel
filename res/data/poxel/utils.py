
def assert_type( value, datatype ):

    if not is_instance( value, datatype ):
        raise TypeError( f"'{ value }' is not type { datatype }." )