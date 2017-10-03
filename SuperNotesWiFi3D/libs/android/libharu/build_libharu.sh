$1/ndk-build clean
$1/ndk-build NDK_DEBUG=0 APP_PLATFORM=android-15 NDK_LIBS_OUT=./sdk_core_bin NDK_OUT=./sdk_core_obj NDK_PROJECT_PATH=$(pwd)
echo "done!"
