# MOOB - Map Optimization and Ongoing Bug-fixing

MOOB is a "polyfill" type mod designed to enhance the usability of the Map Editor in Cities Skylines 2. It addresses key issues and fills in functionality gaps, providing an improved experience for map creation and editing.

## Features
(Thanks to CO's recent updates MOOB only now needs to be used for the editor switch enabling. The previous import/export features are now redundant.)
1. **Enabled Map Editor Option in Game:** Unlocks the map editor within Cities Skylines 2, allowing direct access for map creation and editing.

## Deprecated Features
1. **Improved Heightmap Import:** Utilizes Windows OpenFileDialog for easier heightmap selection and import. (It was easier than using the games ImageAsset functionality)
2. **Heightmap Export to 16-bit RAW:** Enables exporting heightmaps in the 16-bit RAW format for greater precision and compatibility.
3. **Import .PNG, .TIFF or .RAW images:** 16-bit RAW is preferred but if you need to you can use PNG or TIFF formats.
4. **Automatically resize Cities Skylines 1 Heightmaps**: If you import an 1081x1081 heightmap it will resize to an approximate 1:1ish scale to Cities Skylines 1.
5. **8-bit to 16-bit channel conversion**: If you import an 8-bit image it will convert it for you. It will run 10 passes of blurring to try to prevent terracing. (This is WIP and results may not be the best.)

MOOB will be getting new improvements in the near future, so the feature set will expand.

## Installation

### Prerequisites

- Ensure you have BepInEx 5 installed in your Cities Skylines 2 game directory. If not, follow the [BepInEx 5 installation guide](https://github.com/BepInEx/BepInEx).

### Installing MOOB

1. Download the latest version of MOOB from this repository.
2. Place the MOOB plugin file into the `BepInEx/plugins` folder in your Cities Skylines 2 game directory.
3. Launch the game, and MOOB will be active.

## Usage

Once installed, MOOB will automatically enhance the Map Editor's functionality. Open Cities Skylines 2 and access the Map Editor to experience the improvements.

## Contributing

Contributions are welcome! If you have suggestions or fixes, please fork this repository, make your changes, and submit a pull request.

## Support

Encounter an issue or have questions? Please open an issue on this GitHub repository.

## License

MOOB is released under the GNU General Public License v2.0. For more details, see the [LICENSE](LICENSE) file included in this repository.

# Credits
Thank you Dimentox and Captain-Of-Coit for contributions to the Terrain heightmap system and project build system respectively.