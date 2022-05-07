import sys
import poxel

class TestProject ( poxel.Project ):

    def compute( self, frame ):

        output = [
            {
                "type": "image",
                "src": "res/assets/guy.png",
                "position": [ frame, 0, 0 ]
            }
        ]

        return output

TestProject( sys.argv )