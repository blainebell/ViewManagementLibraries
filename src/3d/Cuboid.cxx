#include "Cuboid.h"
#include "Rectangle3.h"
#include "Faces3D.h"

#define CLASSNAME "Cuboid"

int Cuboid::CuboidIndices[CuboidIndicesCnt] = {
    3,2,1,0,-1, // bottom
    4,5,6,7,-1, // top
    1,5,4,0,-1, // left
    2,3,7,6,-1, // right
    2,6,5,1,-1, // front
    0,4,7,3,-1,  // back
};

const string Cuboid::directToString(){
    return "(" + DoubleToString(pt[0]) + "," + DoubleToString(pt[1]) + "," + DoubleToString(pt[2]) + "," + DoubleToString(pt[3]) + "," + DoubleToString(pt[4]) + "," + DoubleToString(pt[5]) + ")";
}

bool Cuboid::setToString(string &str){
    trimString(str, " \t\n()");
    vector<string> vals;
    Tokenize(str, vals, ", ");
    int pl;
    double tmp;
    bool changed = false;
    for (pl = 0; pl < 6 && pl < vals.size() ; pl++){
        tmp = StringToDouble(vals[pl]);
        if (tmp!= pt[pl]){
            changed = true;
        }
        pt[pl] = tmp;
    }
    while (pl < 4){
        if (pt[pl] != 0.){
            changed = true;
        }
        pt[pl++] = 0.;
    }
    return changed;
}

double Cuboid::zero = 0.;

Cuboid::Cuboid() {
  pt[0] = pt[1] = pt[2] = pt[3] = pt[4] = pt[5] = 0.;
}

Cuboid::Cuboid(Rectangle3d &rect) : Cuboid(rect.minx, rect.miny, rect.minz, rect.maxx, rect.maxy, rect.maxz) {
    
}

Cuboid::Cuboid(double *vals) : Cuboid() {
    pt[0]=vals[0]; pt[1]=vals[1]; pt[2] = vals[2]; pt[3] = vals[3]; pt[4] = vals[4]; pt[5] = vals[5];
}

Cuboid::Cuboid(double minx, double miny, double minz, double maxx, double maxy, double maxz) : Cuboid() {
  pt[0]=minx; pt[1]=miny; pt[2] = minz; pt[3] = maxx; pt[4] = maxy; pt[5] = maxz;
}

void Cuboid::cuboidSet(double minX, double minY, double minZ, double maxX, double maxY, double maxZ){
  if (pt[0] != minX || pt[1] != minY || pt[2] != minZ || pt[3] != maxX || pt[4] != maxY || pt[5] != maxZ){
    pt[0] = minX; pt[1] = minY; pt[2] = minZ;
    pt[3] = maxX; pt[4] = maxY; pt[5] = maxZ;
  }
}

bool Cuboid::isInside(double x, double y, double z){
  return (x>=pt[0] && x <=pt[3] && y>=pt[1] && y <= pt[4] && z>=pt[2] && z <= pt[5]);
}

bool Cuboid::isInsideCheckBoth(double x, double y, double z){
  return ((x>=pt[0] && x <= pt[3]) || (x>=pt[3] && x <= pt[0])) &&
         ((y>=pt[1] && y <= pt[4]) || (y>=pt[4] && y <= pt[1])) &&
         ((z>=pt[2] && z <= pt[5]) || (z>=pt[5] && z <= pt[2]));
}

bool Cuboid::add(double newx, double newy, double newz){
    bool ret[] { false };
    double x1 = dpmin(minXget(), newx, ret);
    double y1 = dpmin(minYget(), newy, ret);
    double z1 = dpmin(minZget(), newz, ret);
    
    double x2 = dpmax(maxXget(), newx, ret);
    double y2 = dpmax(maxYget(), newy, ret);
    double z2 = dpmax(maxZget(), newz, ret);
    if (ret[0])
        cuboidSet(x1, y1, z1, x2, y2, z2);
    return ret[0];
}

#define CuboidIndicesCnt 30
int CuboidIndices[CuboidIndicesCnt] = {
    3,2,1,0,-1, // bottom
    4,5,6,7,-1, // top
    1,5,4,0,-1, // left
    2,3,7,6,-1, // right
    2,6,5,1,-1, // front
    0,4,7,3,-1,  // back
};

void addFacesFromVerticesAndIndices(vector<Face *> *faces, float *verts, int *indices, int nidx, Cuboid *bbx = NULL){
  int idxpl = 0;
  Vertex *vhead = NULL, *vnext = NULL;
  while (idxpl < nidx){
    int idx = indices[idxpl];
    if (idx < 0){
      if (vhead){
        vnext->next = vhead;  // circular linked face edges
        // if (!faces) faces = new vector<Face*>();
        faces->push_back(new Face(vhead));
        vnext = vhead = NULL;
      } else {
        cerr << "WARNING: SetFaces3DFromVerticesAndIndicesBBX: vhead=NULL" << endl;
      }
    } else {
      int idxoff = idx * 3;
      if (vhead){
        vnext->next = new Vertex((double)verts[idxoff], (double)verts[idxoff+1], (double)verts[idxoff+2]);
        vnext = vnext->next;
      } else {
        vnext = vhead = new Vertex((double)verts[idxoff], (double)verts[idxoff+1], (double)verts[idxoff+2]);
      }
      if (bbx)
        bbx->add(verts[idxoff], verts[idxoff+1], verts[idxoff+2]);
    }
    idxpl++;
  }
}

vector<Face *> *Cuboid::generateFaces(){
  vector<Face *> *faces = new vector<Face *>();
  Point3d min(pt[0], pt[1], pt[2]);
  Point3d max(pt[3], pt[4], pt[5]);
  float rectverts[] {
    (float)min.x, (float)min.y, (float)min.z,
    (float)min.x, (float)min.y, (float)max.z,
    (float)max.x, (float)min.y, (float)max.z,
    (float)max.x, (float)min.y, (float)min.z,
    (float)min.x, (float)max.y, (float)min.z,
    (float)min.x, (float)max.y, (float)max.z,
    (float)max.x, (float)max.y, (float)max.z,
    (float)max.x, (float)max.y, (float)min.z
  };
  addFacesFromVerticesAndIndices(faces, rectverts, CuboidIndices, CuboidIndicesCnt);
  return faces;
}

