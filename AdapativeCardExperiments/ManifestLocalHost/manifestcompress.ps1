$compress = @{
  Path = "manifest.json", "icon-color.png", "icon-outline.png"
  CompressionLevel = "Fastest"
  DestinationPath = ".\TeamsAppManifest.zip"
}

Compress-Archive @compress -Force