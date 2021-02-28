#!/usr/bin/env bash
WORKSPACE="/Users/scnipper/Documents/unity/Tractor race"
UNITY_BUILD_FOLDER="$WORKSPACE/Build"
FINAL_DIR="$UNITY_BUILD_FOLDER/Android"
FASTLANE_FIN_DIR="$FINAL_DIR/fastlane"
CHANGELOG="$WORKSPACE/CHANGELOG.MD"
FASTLANE_FASTFILE="/Users/scnipper/Documents/fastlane"
# находится в настройках приложения в консоле

APP_ID="1:1033843798164:android:2145a356b4c8fd29dbc481"

if [ -d "$UNITY_BUILD_FOLDER" ]; then
  rm -rf $UNITY_BUILD_FOLDER
fi
mkdir $UNITY_BUILD_FOLDER

##Execute C# Unity CI script that produce Build/ios/*.xcodeproj and Build/android/android.apk
/Applications/Unity/Unity.app/Contents/MacOS/Unity -projectPath $WORKSPACE -batchmode -quit -buildTarget Android -executeMethod AS.Core.Editor.BuildFile.AndroidBuild

## Android
APK_NAME=$(ls $UNITY_BUILD_FOLDER | grep apk)

mkdir $FINAL_DIR
cp "$UNITY_BUILD_FOLDER/$APK_NAME" "$FINAL_DIR/$APK_NAME"
cp -R $FASTLANE_FASTFILE $FASTLANE_FIN_DIR
cd $FINAL_DIR

##Execute Android fastlane with custom options
fastlane android beta apk_path:"$FINAL_DIR/$APK_NAME" release_file:$CHANGELOG app:$APP_ID test_group:"tractor-test"




