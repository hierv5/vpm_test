import bpy
import sys
import argparse
import time

def export_fbx(outPath, collection=''):
    bpy.ops.export_scene.fbx(
        filepath=outPath,
        embed_textures=True,
        path_mode='COPY',
        apply_scale_options='FBX_SCALE_UNITS',
        bake_space_transform=True,
        use_visible=True,
        axis_up='Y',
        collection=collection,
        axis_forward='-Z')

if '__main__' == __name__:
    parser = argparse.ArgumentParser()
    parser.add_argument('--input', type=str, required=True, help='input: Path to input blend file')
    parser.add_argument('--output', type=str, required=True, help='output: Path to output fbx')
    parser.add_argument('--collection', type=str, required=False, default='', help='collection: Collection name to output')

    args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])

    bpy.ops.wm.open_mainfile(filepath=args.input)

    try:
        export_fbx(args.output, args.collection)
    finally:
        time.sleep(1)
