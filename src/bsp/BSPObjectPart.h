
#ifndef CPP_BSPOBJECTPART_H
#define CPP_BSPOBJECTPART_H

#include "Face.h"
#include "Rectangle3.h"
#include "BSPObject.h"

class BSPObjectPart {
public:
    BSPObject *bspObject;
    vector<Face*> *allFaces;
    Rectangle3d bbx;
    BSPObjectPart(BSPObject *, vector<Face*> *);
    ~BSPObjectPart();
    void computeBBX();
    bool split(Plane *plane,
         vector<BSPObjectPart*> *objectsNeg, vector<BSPObjectPart*> *objectsPos,
         vector<BSPObjectPart*> *piecesSameDir, vector<BSPObjectPart*> *piecesOppDir);
    int countNumberOfLines();
    int getNumber();
    void writeLines(float*, int &);
    friend ostream& operator <<(ostream &os,BSPObjectPart &obj);

};

#endif
