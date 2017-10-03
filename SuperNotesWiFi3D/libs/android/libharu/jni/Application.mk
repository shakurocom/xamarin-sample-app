# File: Application.mk

APP_MODULES := haru

# options for release
APP_ABI := armeabi armeabi-v7a mips x86
APP_OPTIM := release
APP_CFLAGS += -O2

# options for development
#APP_ABI := armeabi-v7a
#APP_OPTIM := debug
#APP_CFLAGS += -O0
