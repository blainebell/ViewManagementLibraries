
#include "BSPObjectTree.h"
#include "Cuboid.h"
#include "Faces3D.h"

void addBSPCubeObject(BSPObjectTree &bspTree, Cuboid cube, int id){
  auto faces = cube.generateFaces();
  BSPObject *obj = new BSPObject(id);
  BSPObjectPart *objpart = new BSPObjectPart(obj, faces);
  bspTree.add(obj);
  cout << "faces: " << Faces3D::facesToString(faces) << endl;
}

int main(){
  BSPObjectTree bspTree;

  addBSPCubeObject(bspTree, Cuboid(0,0,0,1,1,1), 1);
  addBSPCubeObject(bspTree, Cuboid(2,2,2,3,3,3), 2);

  bspTree.generate();
  cout << "BSPTree: " << endl << bspTree << endl;
  return 0;
}
