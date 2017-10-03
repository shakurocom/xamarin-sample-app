# File: Android.mk

#traverse all the directory and subdirectory
define walk
  $(wildcard $(1)) $(foreach e, $(wildcard $(1)/*), $(call walk, $(e)))
endef

MY_LOCAL_PATH := $(call my-dir)

###curl prebuilt
# LOCAL_PATH := $(MY_LOCAL_PATH)
# include $(CLEAR_VARS)
# LOCAL_MODULE:= curl-prebuilt
# LOCAL_SRC_FILES := shared/Core/Analytics/curl/lib/android/$(TARGET_ARCH_ABI)/libcurl.a
# include $(PREBUILT_STATIC_LIBRARY)

# ifeq ($(TARGET_ARCH_ABI),armeabi-v7a)
# TURBO_JPEG_ENABLED := 1
# else
# TURBO_JPEG_ENABLED := 0
# endif

### turbojpeg prebuilt - used only with armeabi-v7a
# ifeq ($(TURBO_JPEG_ENABLED),1)
#     LOCAL_PATH := $(MY_LOCAL_PATH)
#     include $(CLEAR_VARS)
#     LOCAL_MODULE:= turbojpeg-prebuilt
#     LOCAL_SRC_FILES := turbojpeg/$(TARGET_ARCH_ABI)/libturbojpeg.a
#     include $(PREBUILT_STATIC_LIBRARY)
# endif

### sdk core

LOCAL_PATH := $(MY_LOCAL_PATH)
include $(CLEAR_VARS)
LOCAL_MODULE    := sdkcore
# LOCAL_STATIC_LIBRARIES := curl-prebuilt

# ifeq ($(TURBO_JPEG_ENABLED),1)
#     LOCAL_STATIC_LIBRARIES += turbojpeg-prebuilt
# endif

#find all the file recursively under jni/

ALLFILES = $(call walk, $(LOCAL_PATH))
FILE_LIST := $(filter %.cpp, $(ALLFILES))
FILE_LIST += $(filter %.c, $(ALLFILES))

# remove libjpeg if turbojpeg is used
# ifeq ($(TURBO_JPEG_ENABLED),1)
#     FILE_LIST := $(foreach f,$(FILE_LIST),$(if $(filter libjpeg,$(subst /, ,$f)),,$f))
#     FILE_LIST := $(foreach f,$(FILE_LIST),$(if $(findstring LSCJpgExportImport.c,$f),,$f))

#     FILE_LIST += turbojpeg/LSCTurboJpgExportImport.c
# endif

# FILE_LIST += JniSdkCoreLibrary.cpp
# FILE_LIST += UUID_c_connector.cpp
# FILE_LIST += TiffLfind.cpp

# ALLFILES = $(call walk, $(LOCAL_PATH)/image_processing)
# FILE_LIST += $(filter %.cpp, $(ALLFILES))
# FILE_LIST += $(filter %.c, $(ALLFILES))

# ALLFILES = $(call walk, $(LOCAL_PATH)/opengl)
# FILE_LIST += $(filter %.cpp, $(ALLFILES))
# FILE_LIST += $(filter %.c, $(ALLFILES))

LOCAL_SRC_FILES := $(FILE_LIST:$(LOCAL_PATH)/%=%)
# LOCAL_SRC_FILES := $(foreach f,$(LOCAL_SRC_FILES),$(if $(findstring xmlhttp,$f),,$f))

# libcurl already contains md5.c
# LOCAL_SRC_FILES := $(foreach f,$(LOCAL_SRC_FILES),$(if $(findstring md5.c,$f),,$f))

ifeq ($(OS),Windows_NT)
   ALLFOLDERS := $(call walk, $(LOCAL_PATH)/shared/Core)
else
   ALLFOLDERS := $(shell find $(LOCAL_PATH) -type d)
endif

# ifeq ($(TURBO_JPEG_ENABLED),1)
#     ALLFOLDERS := $(foreach f,$(ALLFOLDERS),$(if $(filter libjpeg,$(subst /, ,$f)),,$f))
#     ALLFOLDERS += turbojpeg
# endif

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


