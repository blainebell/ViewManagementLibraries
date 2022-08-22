

#ifndef CPP_RECTANGLE3_H
#define CPP_RECTANGLE3_H

#include "Rectangle2.h"
#include "Point3d.h"
#include <sstream>
#include <algorithm>
#include <set>

using namespace std;

template <typename valType> class Rectangle3 : public Rectangle2<valType> {
public:
    static Rectangle3<valType> EMPTY;
    double minz, maxz;
    void set(Rectangle3 &r){
        Rectangle2<valType>::set(r);
        minz = r.minz;
        maxz = r.maxz;
    }
    void set(valType _minx,valType _miny,valType _minz,valType _maxx,valType _maxy,valType _maxz){
        Rectangle2<valType>::minx = _minx; Rectangle2<valType>::miny = _miny; minz = _minz;
        Rectangle2<valType>::maxx = _maxx; Rectangle2<valType>::maxy = _maxy; maxz = _maxz;
    }
    virtual valType area() const {
        valType x = Rectangle2<valType>::maxx-Rectangle2<valType>::minx;
        valType y = Rectangle2<valType>::maxy-Rectangle2<valType>::miny;
        valType z = maxz-minz;
        bool neg = false;
        if (x < 0) neg = !neg;
        if (y < 0) neg = !neg;
        if (z<0) neg = !neg;
        if (neg){
            return (fabs(x*y*z));
        } else {
            return(fabs(x*y*z));
        }
    }
    static bool minx_compare_cnt(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs, int count){
        if (!Rectangle2<valType>::type_compare(lhs.minx, rhs.minx) && count < 6){
            return miny_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.minx < rhs.minx;
    }
    static bool miny_compare_cnt(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs, int count){
        if (!Rectangle2<valType>::type_compare(lhs.miny, rhs.miny) && count < 6){
            return minz_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.miny < rhs.miny;
    }
    static bool minz_compare_cnt(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs, int count){
        if (!Rectangle2<valType>::type_compare(lhs.minz, rhs.minz) && count < 6){
            return maxx_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.minz < rhs.minz;
    }
    static bool maxx_compare_cnt(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs, int count){
        if (!Rectangle2<valType>::type_compare(lhs.maxx, rhs.maxx) && count < 6){
            return maxy_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.maxx < rhs.maxx;
    }
    static bool maxy_compare_cnt(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs, int count){
        if (!Rectangle2<valType>::type_compare(lhs.maxy, rhs.maxy) && count < 6){
            return maxz_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.maxy < rhs.maxy;
    }
    static bool maxz_compare_cnt(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs, int count){
        if (!Rectangle2<valType>::type_compare(lhs.maxz, rhs.maxz) && count < 6){
            return minx_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.maxz < rhs.maxz;
    }
    


    static bool minx_compare(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs){
        if (!Rectangle2<valType>::type_compare(lhs.minx, rhs.minx)){
            return miny_compare_cnt(lhs, rhs, 0);
        }
        return lhs.minx < rhs.minx;
    }
    static bool miny_compare(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs){
        if (!Rectangle2<valType>::type_compare(lhs.miny, rhs.miny)){
            return minz_compare_cnt(lhs, rhs, 0);
        }
        return lhs.miny < rhs.miny;
    }
    static bool minz_compare(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs){
        if (!Rectangle2<valType>::type_compare(lhs.minz, rhs.minz)){
            return maxx_compare_cnt(lhs, rhs, 0);
        }
        return lhs.minz < rhs.minz;
    }
    static bool maxx_compare(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs){
        if (!Rectangle2<valType>::type_compare(lhs.maxx, rhs.maxx)){
            return maxy_compare_cnt(lhs, rhs, 0);
        }
        return lhs.maxx < rhs.maxx;
    }
    static bool maxy_compare(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs){
        if (!Rectangle2<valType>::type_compare(lhs.maxy, rhs.maxy)){
            return maxz_compare_cnt(lhs, rhs, 0);
        }
        return lhs.maxy < rhs.maxy;
    }
    static bool maxz_compare(const Rectangle3<valType>& lhs, const Rectangle3<valType>& rhs){
        if (!Rectangle2<valType>::type_compare(lhs.maxz, rhs.maxz)){
            return minx_compare_cnt(lhs, rhs, 0);
        }
        return lhs.maxz < rhs.maxz;
    }

    bool add(valType _x, valType _y, valType _z){
        bool ret[] = { false };
        double x1 = dpmin(Rectangle2<valType>::minx, _x, ret);
        double y1 = dpmin(Rectangle2<valType>::miny, _y, ret);
        double z1 = dpmin(minz, _z, ret);
        
        double x2 = dpmax(Rectangle2<valType>::maxx, _x, ret);
        double y2 = dpmax(Rectangle2<valType>::maxy, _y, ret);
        double z2 = dpmax(maxz, _z, ret);
        if (ret[0])
            set(x1, y1, z1, x2, y2, z2);
        return ret[0];
    }
    void add(const Point3d &pt){
        add(pt.x, pt.y, pt.z);
    }
    void add(const Rectangle3 &r){
        add(r.minx, r.miny, r.minz);
        add(r.maxx, r.maxy, r.maxz);
    }
    
    void grow(valType _x, valType _y, valType _z){
        set(Rectangle2<valType>::minx-_x, Rectangle2<valType>::miny-_y, minz-_z,
            Rectangle2<valType>::maxx+_x, Rectangle2<valType>::maxy+_y, maxz+_z);
    }
    Rectangle3() : Rectangle2<valType>(), minz(0), maxz(0){}
    virtual ~Rectangle3(){
    }
    Rectangle3(const Rectangle3 &r) : Rectangle2<valType>(r), minz(r.minz), maxz(r.maxz){}
    Rectangle3(Point3d &ptmin, Point3d &ptmax) : Rectangle2<valType>(ptmin.x, ptmin.y, ptmax.x, ptmax.y), minz(ptmin.z), maxz(ptmax.z) {
    }
    Rectangle3(valType _minx,valType _miny,valType _minz,valType _maxx,valType _maxy,valType _maxz) : Rectangle2<valType>(_minx, _miny, _maxx, _maxy), minz(_minz), maxz(_maxz){}
    valType getDepth(){ return maxz-minz; };

    static bool (*default_compare)(const Rectangle3<valType>&, const Rectangle3<valType>&);
    static valType minx_get(const Rectangle3<valType>& rect){ return rect.minx; }
    static valType miny_get(const Rectangle3<valType>& rect){ return rect.miny; }
    static valType maxx_get(const Rectangle3<valType>& rect){ return rect.maxx; }
    static valType maxy_get(const Rectangle3<valType>& rect){ return rect.maxy; }
    static valType minz_get(const Rectangle3<valType>& rect){ return rect.minz; }
    static valType maxz_get(const Rectangle3<valType>& rect){ return rect.maxz; }
    
    virtual const string toString() const {
        stringstream ss;
        ss << "( " << Rectangle2<valType>::minx << ", " << Rectangle2<valType>::miny << ", " << minz <<
              ", " << Rectangle2<valType>::maxx << ", " << Rectangle2<valType>::maxy << ", " << maxz << " )" ;
        return ss.str();
    };
    friend bool operator ==(Rectangle3<valType> &r1, const Rectangle3<valType> &r2){
        return Rectangle2<valType>::type_equals(r1.minx, r2.minx) && Rectangle2<valType>::type_equals(r1.miny, r2.miny) &&
                Rectangle2<valType>::type_equals(r1.maxx, r2.maxx) && Rectangle2<valType>::type_equals(r1.maxy, r2.maxy) &&
                Rectangle2<valType>::type_equals(r1.minz, r2.minz) && Rectangle2<valType>::type_equals(r1.maxz, r2.maxz);
    }

    friend ostream& operator <<(ostream &os, const Rectangle3<valType> &obj){
        os << "( " << obj.minx << ", " << obj.miny << ", " << obj.minz <<
              ", " << obj.maxx << ", " << obj.maxy << ", " << obj.maxz << " )" ;
        return os;
    };

};


template <typename valType>
Rectangle3<valType> consensus(const Rectangle3<valType> &r1, const Rectangle3<valType> &r2, int dimension){
    /* if dimension = 1, rectangles are adjacent on top and bottom,
     if dimension = 0, rectangles are adjacent on left and right */
    if (dimension==2){
        return Rectangle3<valType>(max(r1.minx,r2.minx),
                                   max(r1.miny,r2.miny),
                                   min(r1.minz,r2.minz),
                                   min(r1.maxx,r2.maxx),
                                   min(r1.maxy,r2.maxy),
                                   max(r1.maxz,r2.maxz));
    } else if (dimension==1){
        return Rectangle3<valType>(max(r1.minx,r2.minx),
                                   min(r1.miny,r2.miny),
                                   max(r1.minz,r2.minz),
                                   min(r1.maxx,r2.maxx),
                                   max(r1.maxy,r2.maxy),
                                   min(r1.maxz,r2.maxz));
    } else if (dimension==0){
        return Rectangle3<valType>(min(r1.minx,r2.minx),
                                   max(r1.miny,r2.miny),
                                   max(r1.minz,r2.minz),
                                   max(r1.maxx,r2.maxx),
                                   min(r1.maxy,r2.maxy),
                                   min(r1.maxz,r2.maxz));
    }
    
    stringstream ss;
    ss << "WARNING: consensus called dimension=" << dimension << endl;
#ifndef NO_DP
    if (staticDataManagerInstance)
        staticDataManagerInstance->dperr(ss.str().c_str());
    else
#endif
        cerr << ss.str();
    return Rectangle3<valType>();
}

template <typename valType>
bool intersect(Rectangle3<valType> &src1, Rectangle3<valType> &src2, Rectangle3<valType> &dest) {
    valType x1 = max(src1.minx, src2.minx);
    valType y1 = max(src1.miny, src2.miny);
    valType z1 = max(src1.minz, src2.minz);
    valType x2 = min(src1.maxx, src2.maxx);
    valType y2 = min(src1.maxy, src2.maxy);
    valType z2 = min(src1.maxz, src2.maxz);
    dest.set(x1,y1,z1,x2,y2,z2);
    return (dest.getWidth()>0 && dest.getHeight()>0 && dest.getDepth()>0);
}

template <typename valType>
bool has_intersection(Rectangle3<valType> &src1, Rectangle3<valType> &src2){
    Rectangle3<valType> dest;
    return intersect(src1, src2, dest);
}

template <typename valType> Rectangle3<valType>
Rectangle3<valType>::EMPTY(Rectangle2<valType>::MAX_VAL, Rectangle2<valType>::MAX_VAL,
                           Rectangle2<valType>::MAX_VAL, Rectangle2<valType>::MIN_VAL,
                           Rectangle2<valType>::MIN_VAL, Rectangle2<valType>::MIN_VAL);

template <typename valType> class FullRectangle3 : public Rectangle3<valType> {
 public:
    int key;
    
    FullRectangle3() : Rectangle3<valType>(), key(0) {}
    FullRectangle3(int keyid, valType _minx,valType _miny,valType _minz,valType _maxx,valType _maxy,valType _maxz) : Rectangle3<valType>(_minx, _miny, _minz, _maxx, _maxy, _maxz), key(keyid) {
    }
    FullRectangle3(valType _minx,valType _miny,valType _minz,valType _maxx,valType _maxy,valType _maxz) : Rectangle3<valType>(_minx, _miny, _minz, _maxx, _maxy, _maxz), key(0) {}
    FullRectangle3(Rectangle3<valType> r) : Rectangle3<valType>(r), key(0){}
    static valType minx_get(const FullRectangle3<valType>& rect){ return rect.minx; }
    static valType miny_get(const FullRectangle3<valType>& rect){ return rect.miny; }
    static valType maxx_get(const FullRectangle3<valType>& rect){ return rect.maxx; }
    static valType maxy_get(const FullRectangle3<valType>& rect){ return rect.maxy; }
    static valType minz_get(const FullRectangle3<valType>& rect){ return rect.minz; }
    static valType maxz_get(const FullRectangle3<valType>& rect){ return rect.maxz; }
};

//typedef bool(*Rectangle3dComparator) (const Rectangle3d &lhs,const Rectangle3d &rhs);
//typedef set<const Rectangle3d, Rectangle3dComparator> Rectangles3D;

typedef Rectangle3<double> Rectangle3d;
typedef Rectangle3<int> Rectangle3i;
typedef FullRectangle3<double> FullRectangle3d;

typedef bool(*Rectangle3iComparator) (const Rectangle3i &lhs,const Rectangle3i &rhs);
typedef set<Rectangle3i, Rectangle3iComparator> Rectangles3I;

typedef bool(*Rectangle3dComparator) (const Rectangle3d &lhs, const Rectangle3d &rhs);
typedef set<Rectangle3d, Rectangle3dComparator> Rectangles3D;

typedef set<FullRectangle3d, Rectangle3dComparator> FullRectangles3D;


#endif
