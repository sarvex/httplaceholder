#!/bin/bash
VERSION="$1"
if [ "$VERSION" = "" ]; then
  echo "Version not set"
  exit 1
fi

NUGET_API_KEY="$2"
if [ "$NUGET_API_KEY" = "" ]; then
  echo "Nuget API key not set"
  exit 1
fi

dotnet nuget push dist/HttPlaceholder.$VERSION.nupkg -k $NUGET_API_KEY -s https://api.nuget.org/v3/index.json
dotnet nuget push dist/HttPlaceholder.Client.$VERSION.nupkg -k $NUGET_API_KEY -s https://api.nuget.org/v3/index.json