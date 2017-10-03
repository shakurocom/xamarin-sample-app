# File: Android.mk

#traverse all the directory and subdirectory
define walk
  $(wildcard $(1)) $(foreach e, $(wildcard $(1)/*), $(call walk, $(e)))
endef

NDK_PROJECT_PATH := ./
MY_LOCAL_PATH := $(call my-dir)


### sdk core

LOCAL_PATH := $(MY_LOCAL_PATH)
include $(CLEAR_VARS)
LOCAL_MODULE    := haru

#find all the file recursively under jni/

ALLFILES = $(call walk, $(LOCAL_PATH))
FILE_LIST := $(filter %.cpp, $(ALLFILES))
FILE_LIST += $(filter %.c, $(ALLFILES))

LOCAL_SRC_FILES := $(FILE_LIST:$(LOCAL_PATH)/%=%)

ifeq ($(OS),Windows_NT)
   ALLFOLDERS := $(call walk, $(LOCAL_PATH)/shared/Core)
else
   ALLFOLDERS := $(shell find $(LOCAL_PATH) -type d)
endif

LOCAL_C_INCLUDES := $(ALLFOLDERS)

LOCAL_LDLIBS := -llog -lz -ldl #-ljnigraphics -lEGL  -lGLESv2
# LOCAL_LDFLAGS += -ljnigraphics
# for release
LOCAL_CFLAGS := -frtti -std=c99 -DNDEBUG
# for development
#LOCAL_CFLAGS := -std=c99 -funwind-tables -Wl,--no-merge-exidx-entries -DFLAG_DBG -fno-inline

# ifeq ($(TURBO_JPEG_ENABLED),1)
#     LOCAL_CFLAGS += -DTURBO_JPEG_ENABLED
# endif

include $(BUILD_SHARED_LIBRARY)


