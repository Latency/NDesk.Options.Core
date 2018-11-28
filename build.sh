#!/usr/bin/env bash

if [[ !$1 ]]; then
    CONFIGURATION="Debug"
fi

if [[ $1 ]]; then
    CONFIGURATION=$1
fi

dotnet restore
dotnet build ./src -c $CONFIGURATION
dotnet build ./test -c $CONFIGURATION
