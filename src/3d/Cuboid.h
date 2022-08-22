#ifndef CPP_CUBOIDD_H
#define CPP_CUBOIDD_H

#include "stlTools.h"
#include "Point3d.h"
#include "Rectangle3.h"
#include "Face.h"
#include <cmath>

#define CuboidIndicesCnt 30
class Cuboid {
 public:
  static double zero;
  static int CuboidIndices[CuboidIndicesCnt];

  double pt[6]; /* minx, miny, minz, maxx, maxy, maxz */
  Cuboid();
  Cuboid(Rectangle3d&);
  Cuboid(double minx, double miny, double minz, double maxx, double maxy, double maxz);
  Cuboid(double *vals);
  virtual string getClass(){ return "Cuboid"; }
#ifndef NO_DP
  virtual void valueObjectSet(KBObject *obj);
#endif
    virtual void valueSet(double x, double y, double z, double mx, double my, double mz){ pt[0]=x; pt[1]=y; pt[2]=z; pt[3]=mx; pt[4]=my; pt[5]=mz; }
#ifndef NO_DP
  virtual void valueSet(KBData &value);
#endif
  virtual void resetValue(){}
  virtual const string directToString();
  virtual const string toString(){ return directToString(); } //return ("CuboidD (" + DoubleToString(pt[0]) + "," + DoubleToString(pt[1]) + "," + DoubleToString(pt[2]) + "," + DoubleToString(pt[3]) + "," + DoubleToString(pt[4]) + "," + DoubleToString(pt[5]) + ")"); }
  friend ostream& operator <<(ostream &os, Cuboid &obj);
#ifndef NO_DP
  static KBObject *createCuboid(KBObject*);
  virtual KBObject *duplicate(){
    return createCuboid(this);
  };
#endif
  virtual double minXget(){ return pt[0]; }
  virtual double minYget(){ return pt[1]; }
  virtual double minZget(){ return pt[2]; }
  virtual double maxXget(){ return pt[3]; }
  virtual double maxYget(){ return pt[4]; }
  virtual double maxZget(){ return pt[5]; }
#ifndef NO_DP
  virtual KBObject *hasAreaObj(){ return new KBBoolean(area() > TOLER); }
  virtual KBObject *minXgetObj(){ return new KBDouble(pt[0]); }
  virtual KBObject *minYgetObj(){ return new KBDouble(pt[1]); }
  virtual KBObject *minZgetObj(){ return new KBDouble(pt[2]); }
  virtual KBObject *maxXgetObj(){ return new KBDouble(pt[3]); }
  virtual KBObject *maxYgetObj(){ return new KBDouble(pt[4]); }
  virtual KBObject *maxZgetObj(){ return new KBDouble(pt[5]); }
#endif
  virtual void minXset(double minX){ pt[0] = minX; }
  virtual void minYset(double minY){ pt[1] = minY; }
  virtual void minZset(double minZ){ pt[2] = minZ; }
  virtual void maxXset(double maxX){ pt[3] = maxX; }
  virtual void maxYset(double maxY){ pt[4] = maxY; }
  virtual void maxZset(double maxZ){ pt[5] = maxZ; }
  virtual double getWidth(){ return pt[3] - pt[0]; } 
  virtual double getHeight(){ return pt[4] - pt[1]; } 
  virtual double getDepth(){ return pt[5] - pt[2]; }
  virtual double getCenterX(){ return ((pt[3] + pt[0])/2.); } 
  virtual double getCenterY(){ return ((pt[4] + pt[1])/2.); } 
  virtual double getCenterZ(){ return ((pt[5] + pt[2])/2.); }

#ifndef NO_DP
  virtual KBObject *getWidthObj(){ return new KBDouble(pt[3] - pt[0]); }
  virtual KBObject *getHeightObj(){ return new KBDouble(pt[4] - pt[1]); }
  virtual KBObject *getDepthObj(){ return new KBDouble(pt[5] - pt[2]); }
  virtual KBObject *getCenterXObj(){ return new KBDouble(((pt[3] + pt[0])/2.)); }
  virtual KBObject *getCenterYObj(){ return new KBDouble(((pt[4] + pt[1])/2.)); }
  virtual KBObject *getCenterZObj(){ return new KBDouble(((pt[5] + pt[2])/2.)); }

  virtual KBObject *getCenterObj() { return new Point3d(getCenterX(), getCenterY(), getCenterZ()); }
  virtual KBObject *getSizeObj() { return new Point3d(getWidth(), getHeight(), getDepth()); }
#endif
  virtual void cuboidSet(double minX, double minY, double minZ, double maxX, double maxY, double maxZ);
  virtual bool isInside(double x, double y, double z);
  virtual bool isInsideCheckBoth(double x, double y, double z);
  virtual bool setToString(string &);
  virtual double &operator[](int d){ if (d>=0 && d<6){ return pt[d]; } else { return zero;} }
  virtual bool add(double x, double y, double z);
#ifndef NO_DP
  virtual void add(KBObject *);
  virtual KBObject *directToStringObj();
#endif
  vector<Face *> *generateFaces();
};

#ifndef NO_DP
void setCuboid(KBObject *obj, Cuboid *ptobj);
#endif

#endif
