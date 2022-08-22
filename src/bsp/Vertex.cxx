

#include "Vertex.h"

#include <math.h>
#include "stlTools.h"
#include <sstream>

using namespace std;

double Vertex::length(){
    return pow(x*x + y*y + z*z, .5);
}

Vertex::Vertex() : Point3d(0.,0.,0.),/*userData(NULL),*/ next(NULL) {
}

Vertex::Vertex(double x, double y, double z) : Point3d(x,y,z), /*userData(NULL),*/ next(NULL) {
}

Vertex::Vertex( string vstr) : Point3d(0.,0.,0.),/*userData(NULL),*/ next(NULL) {
    vector<string> vals;
    trimString(vstr, "()");
    Tokenize(vstr, vals, ",", false, true);
    if (vals.size()>0) x = StringToDouble(vals[0]);
    if (vals.size()>1) y = StringToDouble(vals[1]);
    if (vals.size()>2) z = StringToDouble(vals[2]);
}
Vertex::Vertex(Vertex *v2) : /*userData(NULL),*/ next(NULL) {
    if (v2){
        x = v2->x;
        y = v2->y;
        z = v2->z;
    }
}

bool Vertex::equals(const Vertex *v2){
    return doubleIsEqual(v2->x, x ) &&
        doubleIsEqual(v2->y, y ) &&
        doubleIsEqual(v2->z, z );
}

string Vertex::toString(){
    stringstream ss;
    ss << "(" << x << "," << y << "," << z << ")" ;
    return ss.str();
}

#define DEBUG_STREAM

ostream& operator <<(ostream &os, Vertex &obj){
    os << obj.toString();
    return (os);
}
