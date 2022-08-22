
#ifndef CPP_DATALISTINTERFACE_H
#define CPP_DATALISTINTERFACE_H

class DataListInterfaceClearable {
public:
    virtual void clearAllData() = 0;
};

template <typename nodeClass> class DataListInterface : public DataListInterfaceClearable  {
public:
    DataListInterface() : DataListInterfaceClearable() {}
    virtual void addData(const nodeClass&) = 0;
    virtual void addDataAt(nodeClass&, int at) = 0;
    virtual void deleteData(nodeClass&) = 0;
};

#endif
