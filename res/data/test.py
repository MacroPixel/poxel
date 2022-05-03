import sys
import poxel

my_project = poxel.Project()

temp_elements = [ poxel.Element() for _ in range( 10 ) ]
print( [ el.id for el in temp_elements ] )

my_project.run( sys.argv )