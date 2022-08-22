
UNAME := $(shell uname)
UNAME_P := $(shell uname -p)
O = o
OBJ_DIR = obj
CXX = g++
ARGS = -std=c++11 -Wno-c++11-extensions

.PRECIOUS: %/.mkdir

ALL_BSP = $(addprefix $(OBJ_DIR)/bsp/,$(shell cd src/bsp; ls *.cxx | sed -e 's/\.cxx/.o/g'))
ALL_SM = $(addprefix $(OBJ_DIR)/sm/,$(shell cd src/sm; ls *.cxx | sed -e 's/\.cxx/.o/g'))
ALL_3D = $(addprefix $(OBJ_DIR)/3d/,$(shell cd src/3d; ls *.cxx | sed -e 's/\.cxx/.o/g'))
ALL_SHARED = $(addprefix $(OBJ_DIR)/shared/,$(shell cd src/shared; ls *.cxx | sed -e 's/\.cxx/.o/g'))

ALL_OBJ_FILES = $(ALL_BSP) $(ALL_SM) $(ALL_3D) $(ALL_SHARED)

$(warning ALL_OBJ_FILES: $(ALL_OBJ_FILES))

%/.mkdir:
	echo MKDIR ${@D}
	mkdir -p ${@D}
	( [ ! -e $@ ] && touch $@ )

$(OBJ_DIR)/bsp/%.$O: src/bsp/%.cxx $(OBJ_DIR)/bsp/.mkdir
	$(CXX) -c $(ARGS) -I. -Isrc/bsp -Isrc/sm -Isrc/shared -Isrc/3d -DNO_DP -o $@ $<

$(OBJ_DIR)/sm/%.$O: src/sm/%.cxx $(OBJ_DIR)/sm/.mkdir
	$(CXX) -c $(ARGS) -I. -Isrc/bsp -Isrc/sm -Isrc/shared -Isrc/3d -DNO_DP -o $@ $<

$(OBJ_DIR)/3d/%.$O: src/3d/%.cxx $(OBJ_DIR)/3d/.mkdir
	$(CXX) -c $(ARGS) -I. -Isrc/bsp -Isrc/sm -Isrc/shared -Isrc/3d -DNO_DP -o $@ $<

$(OBJ_DIR)/shared/%.$O: src/shared/%.cxx $(OBJ_DIR)/shared/.mkdir
	$(CXX) -c $(ARGS) -I. -Isrc/bsp -Isrc/sm -Isrc/shared -Isrc/3d -DNO_DP -o $@ $<

#bsp: $(ALL_OBJ_FILES) bin/.mkdir
#	g++ -std=c++11 -Wno-c++11-extensions -Isrc/bsp -Isrc/sm -Isrc/shared src/main.cxx -DNO_DP -o bin/$@ $<

all: testbsp testsm

bin/testbsp: $(ALL_OBJ_FILES) src/testbsp.cxx bin/.mkdir
	g++ -std=c++11 -Wno-c++11-extensions -Isrc/bsp -Isrc/3d -Isrc/sm -Isrc/shared src/testbsp.cxx -DNO_DP -o bin/testbsp $(ALL_OBJ_FILES)

bin/testsm: $(ALL_OBJ_FILES) src/testsm.cxx bin/.mkdir
	g++ -std=c++11 -Wno-c++11-extensions -Isrc/bsp -Isrc/3d -Isrc/sm -Isrc/shared src/testsm.cxx -DNO_DP -o bin/testsm $(ALL_OBJ_FILES)

clean:
	rm -rf obj bin

testbsp: $(ALL_OBJ_FILES) bin/testbsp

testsm: $(ALL_OBJ_FILES) bin/testsm

