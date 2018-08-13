import maya.cmds as cmds

def printRotations(root, depth=0):
    rotation = cmds.xform(root, q=True, rotation=True)
    indent = '\t' * depth;
    print indent, root, rotation[0], -rotation[1], -rotation[2]
    rels = cmds.listRelatives(root, c=True, path=True, typ="transform")
    depth += 1
    if rels is None:
        return
    for ctrl in rels:
        printRotations(ctrl, depth)
    
printRotations(cmds.ls(sl=True))
