
#ifndef CPP_DATALISTINTERFACE_H
#define CPP_DATALISTINTERFACE_H

#ifndef NO_DP
#include "KBData.h"
#include "NeedDataManager.h"
#endif

class DataListInterfaceClearable {
public:
    virtual void clearAllData() = 0;
};
#ifndef NO_DP
class DataListInterfaceParent : public DataListInterfaceClearable, public KBData, public NeedDataManager {
public:
    DataListInterfaceParent();
    virtual void addData(KBObject *) = 0;
    virtual void addDataAt(KBData *, int at) = 0;
    virtual void deleteData(KBObject *) = 0;
    virtual void addDataAtKB(KBObject *d, KBObject *at){
        addDataAt(dynamic_cast<KBData*>(resolveKBObject(d)), resolveKBInteger(at));
    }
};
#endif

template <typename nodeClass> class DataListInterface : public DataListInterfaceClearable  {
public:
    DataListInterface() : DataListInterfaceClearable() {}
    virtual void addData(const nodeClass&) = 0;
    virtual void addDataAt(nodeClass&, int at) = 0;
    virtual void deleteData(nodeClass&) = 0;
};

#endif
