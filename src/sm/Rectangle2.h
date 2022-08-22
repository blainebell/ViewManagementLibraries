

#ifndef CPP_RECTANGLE2D_H
#define CPP_RECTANGLE2D_H

#include "stlTools.h"
#include <math.h>

#ifndef NO_DP
#include "DataManagerInstance.h"
extern DataManagerInstance *staticDataManagerInstance;
#else
#include <sstream>
#endif
#include "Point2d.h"

using namespace std;

template <typename valType> class Rectangle2 {
public:
    static Rectangle2<valType> EMPTY;
    static valType MIN_VAL;
    static valType MAX_VAL;
    static int type_compare(const valType&, const valType&);
    static bool type_equals(const valType&, const valType&);
    valType minx, miny, maxx, maxy;
    Rectangle2() : minx (0), miny(0), maxx(0), maxy(0) {}
    Rectangle2(const Rectangle2 &r) : minx (r.minx), miny(r.miny), maxx(r.maxx), maxy(r.maxy) {}
    Rectangle2(valType _minx,valType _miny,valType _maxx,valType _maxy) : minx(_minx), miny(_miny), maxx(_maxx), maxy(_maxy) {}
    void set(valType _minx,valType _miny,valType _maxx,valType _maxy){
        minx = _minx;
        miny = _miny;
        maxx = _maxx;
        maxy = _maxy;
    }
    valType getWidth() const { return maxx-minx; };
    valType getHeight() const { return maxy-miny; };
    void set(Rectangle2 &r){
        minx = r.minx; miny = r.miny;
        maxx = r.maxx; maxy = r.maxy;
    }
    virtual valType area() const {
        valType x = maxx-minx;
        valType y = maxy-miny;
        if (x < 0 || y < 0){
            return ((valType)fabs(x*y));
        } else {
            return((valType)fabs(x*y));
        }
    }
    virtual void grow(valType x, valType y){
        set(minx - x, miny - y, maxx + x, maxy + y);
    }
    virtual double getCenterX() const { return (minx + maxx)/2.; }
    virtual double getCenterY() const { return (miny + maxy)/2.; }
    virtual bool enclosedBy(const Rectangle2 *r) const {
        if (r->minx <= minx &&
            r->maxx >= maxx &&
            r->miny <= miny &&
            r->maxy >= maxy){
            return true;
        }
        return false;
    }
    virtual bool isInside(valType x, valType y){
        return (x>=minx && x <=maxx &&
                y>=miny && y <=maxy);
    }
    virtual bool enclosedBy(const Rectangle2 r) const {
        return enclosedBy(&r);
    }
    static bool minx_compare_cnt(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs, int count){
        if (!type_compare(lhs.minx, rhs.minx) && count < 4){
            return miny_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.minx < rhs.minx;
    }
    static bool miny_compare_cnt(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs, int count){
        if (!type_compare(lhs.miny, rhs.miny) && count < 4){
            return maxx_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.miny < rhs.miny;
    }
    static bool maxx_compare_cnt(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs, int count){
        if (!type_compare(lhs.maxx, rhs.maxx) && count < 4){
            return maxy_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.maxx < rhs.maxx;
    }
    static bool maxy_compare_cnt(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs, int count){
        if (!type_compare(lhs.maxy, rhs.maxy) && count < 4){
            return minx_compare_cnt(lhs, rhs, ++count);
        }
        return lhs.maxy < rhs.maxy;
    }
    
    static bool minx_compare(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs){
        if (!type_compare(lhs.minx, rhs.minx)){
            return miny_compare_cnt(lhs, rhs, 0);
        }
        return lhs.minx < rhs.minx;
    }
    static bool miny_compare(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs){
        if (!type_compare(lhs.miny, rhs.miny)){
            return maxx_compare_cnt(lhs, rhs, 0);
        }
        return lhs.miny < rhs.miny;
    }
    static bool maxx_compare(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs){
        if (!type_compare(lhs.maxx, rhs.maxx)){
            return maxy_compare_cnt(lhs, rhs, 0);
        }
        return lhs.maxx < rhs.maxx;
    }
    static bool maxy_compare(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs){
        if (!type_compare(lhs.maxy, rhs.maxy)){
            return minx_compare_cnt(lhs, rhs, 0);
        }
        return lhs.maxy < rhs.maxy;
    }
    
    static bool area_compare(const Rectangle2<valType>& lhs, const Rectangle2<valType>& rhs){
        auto lhsarea = lhs.area();
        auto rhsarea = rhs.area();
        if (!type_compare(lhsarea, rhsarea)){
            return minx_compare(lhs, rhs);
        }
        return lhsarea > rhsarea;
    }

    static bool (*default_compare)(const Rectangle2<valType>&, const Rectangle2<valType>&);
    static valType minx_get(const Rectangle2<valType>& rect){ return rect.minx; }
    static valType miny_get(const Rectangle2<valType>& rect){ return rect.miny; }
    static valType maxx_get(const Rectangle2<valType>& rect){ return rect.maxx; }
    static valType maxy_get(const Rectangle2<valType>& rect){ return rect.maxy; }
    bool add(valType _x, valType _y){
        bool ret[] = { false };
        double x1 = dpmin(minx, _x, ret);
        double y1 = dpmin(miny, _y, ret);
        double x2 = dpmax(maxx, _x, ret);
        double y2 = dpmax(maxy, _y, ret);
        if (ret[0])
            set(x1, y1, x2, y2);
        return ret[0];
    }

    virtual const string toString() const {
        stringstream ss;
        ss << "( " << minx << ", " << miny << ", " << maxx << ", " << maxy << " )" ;
        return ss.str();
    };
    friend bool operator ==(Rectangle2<valType> &r1, const Rectangle2<valType> &r2){
        return type_equals(r1.minx, r2.minx) && type_equals(r1.miny, r2.miny) &&
        type_equals(r1.maxx, r2.maxx) && type_equals(r1.maxy, r2.maxy);
    }
    friend bool operator !=(Rectangle2<valType> &r1, const Rectangle2<valType> &r2){
        return !(type_equals(r1.minx, r2.minx) && type_equals(r1.miny, r2.miny) &&
                 type_equals(r1.maxx, r2.maxx) && type_equals(r1.maxy, r2.maxy));
    }
    friend ostream& operator <<(ostream &os, const Rectangle2<valType> &obj){
        os << "( " << obj.minx << ", " << obj.miny << ", " << obj.maxx << ", " << obj.maxy << " )" ;
        return os;
    };
    /*
    friend bool intersects(const Rectangle2<valType> &src1, const Rectangle2<valType> &src2){
        valType x1 = max(src1.minx, src2.minx);
        valType y1 = max(src1.miny, src2.miny);
        valType x2 = min(src1.maxx, src2.maxx);
        valType y2 = min(src1.maxy, src2.maxy);
        Rectangle2<valType> dest(x1,y1,x2,y2);
        bool ret = dest.getWidth()>0 && dest.getHeight()>0;
        if (ret){
            return (true);
        } else if (type_equals(src1.area(),0)){
            return (src1.enclosedBy(&dest));
        } else if (type_equals(src2.area(),0)){
            return (src2.enclosedBy(&dest));
        }
        return (false);
    };*/
    float distanceOfCenterFromPointLength(float x, float y){
        float rx = getCenterX()-x, ry = getCenterY()-y;
        return (sqrt(rx*rx + ry*ry));
    }
    float distanceFromPoint(float ptx, float pty){
        bool left = maxx<ptx;
        bool right = minx>ptx;
        bool bottom = maxy<pty;
        bool top = miny>pty;
        if (!left && !right && !top && !bottom){
            return (0.);
        }
        if (!left && !right){
            if (top){
                return (miny-pty);
            } else {
                return (pty-maxy);
            }
        }
        if (!bottom && !top){
            if (right){
                return (minx-ptx);
            } else {
                return (ptx-maxx);
            }
        }
        float bv[2][2];//=new double[2][2];
        if (top){
            bv[0][1]=miny;
            bv[1][1]=pty;
        } else {
            bv[0][1]=maxy;
            bv[1][1]=pty;
        }
        if (right){
            bv[0][0]=minx;
            bv[1][0]=ptx;
        }else {
            bv[0][0]=maxx;
            bv[1][0]=ptx;
        }
        return (sqrtf(pow(bv[1][0]-bv[0][0],2.) + pow(bv[1][1]-bv[0][1],2.)));
    }
};

template <typename valType> class FullRectangle2 : public Rectangle2<valType> {
public:
    long key;
    
    FullRectangle2() : Rectangle2<valType>(), key(0) {}
    FullRectangle2(long keyid, valType _minx,valType _miny,valType _maxx,valType _maxy) : Rectangle2<valType>(_minx, _miny, _maxx, _maxy), key(keyid) {
    }
    FullRectangle2(valType _minx,valType _miny,valType _maxx,valType _maxy) : Rectangle2<valType>(_minx, _miny, _maxx, _maxy), key(0) {}
    FullRectangle2(Rectangle2<valType> r) : Rectangle2<valType>(r), key(0){}
    FullRectangle2(long keyval, Rectangle2<valType> r) : Rectangle2<valType>(r), key(keyval){}
    static valType minx_get(const FullRectangle2<valType>& rect){ return rect.minx; }
    static valType miny_get(const FullRectangle2<valType>& rect){ return rect.miny; }
    static valType maxx_get(const FullRectangle2<valType>& rect){ return rect.maxx; }
    static valType maxy_get(const FullRectangle2<valType>& rect){ return rect.maxy; }
    virtual const string toString() const {
        stringstream ss;
        ss << "( " << key << ", " << Rectangle2<valType>::minx << ", " << Rectangle2<valType>::miny << ", " <<
                        Rectangle2<valType>::maxx << ", " << Rectangle2<valType>::maxy << " )" ;
        return ss.str();
    };
    friend ostream& operator <<(ostream &os, const FullRectangle2<valType> &obj){
        os << "( " << obj.key << ", " << obj.minx << ", " << obj.miny << ", " << obj.maxx << ", " << obj.maxy << " )" ;
        return os;
    };
};

template <typename valType>
bool intersect(const Rectangle2<valType> &src1, const Rectangle2<valType> &src2, Rectangle2<valType> &dest) {
    valType x1 = max(src1.minx, src2.minx);
    valType y1 = max(src1.miny, src2.miny);
    valType x2 = min(src1.maxx, src2.maxx);
    valType y2 = min(src1.maxy, src2.maxy);
    dest.set(x1,y1,x2,y2);
    return (dest.getWidth()>0 && dest.getHeight()>0);
}

template <typename valType>
bool intersect_check(const Rectangle2<valType> &src1, const Rectangle2<valType> &src2, Rectangle2<valType> &dest) {
    if (src1.area() <= 0 || src2.area() <= 0)
        return false;
    return intersect(src1,src2,dest);
}

template <typename valType>
void relativeTo(const Rectangle2<valType> &src1, const Point2d &srcpt, Point2d &dest) {
    dest.set( (srcpt.x-src1.minx) / (double)src1.getWidth(),
              (srcpt.y-src1.miny) / (double)src1.getHeight());
}

template <typename valType>
void relativeFrom(const Rectangle2<valType> &src1, const Point2d &srcpt, Point2d &dest) {
    dest.set((srcpt.x * src1.getWidth()) + src1.minx,
             (srcpt.y * src1.getHeight()) + src1.miny );
}

template <typename valType>
bool has_intersection(const Rectangle2<valType> &src1, const Rectangle2<valType> &src2){
    Rectangle2<valType> dest;
    return intersect(src1, src2, dest);
}

template <typename valType>
bool has_intersection_check(const Rectangle2<valType> &src1, const Rectangle2<valType> &src2){
    Rectangle2<valType> dest;
    return intersect_check(src1, src2, dest);
}

template <typename valType>
bool isAdjacentOnLeftOf(const Rectangle2<valType> &r1, const Rectangle2<valType> &r2){
    if (r1.miny >= r2.maxy || r1.maxy <= r2.miny){
        return (false);
    }
    if (Rectangle2<valType>::type_equals(r1.maxx, r2.minx)){
        return (true);
    }
    return (false);
}

template <typename valType>
bool isAdjacentOnRightOf(const Rectangle2<valType> &r1, const Rectangle2<valType> &r2){
    if (r1.miny >= r2.maxy || r1.maxy <= r2.miny){
        return (false);
    }
    if (Rectangle2<valType>::type_equals(r1.minx, r2.maxx)){
        return (true);
    }
    return (false);
}

template <typename valType>
bool isAdjacentX(const Rectangle2<valType> &r1, const Rectangle2<valType> &r2){
    if (r1.miny >= r2.maxy || r1.maxy <= r2.miny){
        return (false);
    }
    if (Rectangle2<valType>::type_equals(r1.minx, r2.maxx) ||
        Rectangle2<valType>::type_equals(r1.maxx, r2.minx)){
        return (true);
    }
    return (false);
}

template <typename valType>
bool isAdjacentY(const Rectangle2<valType> &r1, const Rectangle2<valType> &r2){
    if (r1.minx >= r2.maxx || r1.maxx <= r2.minx){
        return (false);
    }
    if (Rectangle2<valType>::type_equals(r1.miny, r2.maxy) ||
        Rectangle2<valType>::type_equals(r1.maxy, r2.miny)){
        return (true);
    }
    return (false);
}

template <typename valType>
bool isAdjacentOnTopOf(const Rectangle2<valType> &r1, const Rectangle2<valType> &r2){
    if (r1.minx >= r2.maxx || r1.maxx <= r2.minx){
        return (false);
    }
    if (Rectangle2<valType>::type_equals(r1.miny, r2.maxy)){
        return (true);
    }
    return (false);
}

template <typename valType>
bool isAdjacentOnBottomOf(const Rectangle2<valType> &r1, const Rectangle2<valType> &r2){
    if (r1.minx >= r2.maxx || r1.maxx <= r2.minx){
        return (false);
    }
    if (Rectangle2<valType>::type_equals(r1.maxy, r2.miny)){
        return (true);
    }
    return (false);
}

template <typename valType>
Rectangle2<valType> consensus(const Rectangle2<valType> &r1, const Rectangle2<valType> &r2, int dimension){
    /* if dimension = 1, rectangles are adjacent on top and bottom,
       if dimension = 0, rectangles are adjacent on left and right */
    if (dimension==1){
        return Rectangle2<valType>(max(r1.minx,r2.minx),
                                   min(r1.miny,r2.miny),
                                   min(r1.maxx,r2.maxx),
                                   max(r1.maxy,r2.maxy));
    } else if (dimension==0){
        return Rectangle2<valType>(min(r1.minx,r2.minx),
                                   max(r1.miny,r2.miny),
                                   max(r1.maxx,r2.maxx),
                                   min(r1.maxy,r2.maxy));
    }
    stringstream ss;
    ss << "WARNING: consensus called dimension=" << dimension << endl;
#ifndef NO_DP
	if (staticDataManagerInstance)
        staticDataManagerInstance->dperr(ss.str().c_str());
    else
#endif
        cerr << ss.str();
    return Rectangle2<valType>();
}

template <typename valType>
float getDistanceBetween(const Rectangle2<valType> &rect1, const Rectangle2<valType> &rect2){
    bool left,right,top,bottom;
    left = rect1.maxx<rect2.minx;
    right = rect1.minx>rect2.maxx;
    bottom = rect1.maxy<rect2.miny;
    top = rect1.miny>rect2.maxy;
    if (!left && !right && !top && !bottom){
        return (0);
    }
    if (!left && !right){
        if (top){
            return (rect1.miny-rect2.maxy);
        } else {
            return (rect2.miny-rect1.maxy);
        }
    }
    if (!bottom && !top){
        if (right){
            return (rect1.minx-rect2.maxx);
        } else {
            return (rect2.minx-rect1.maxx);
        }
    }
    float bv[2][2];
    if (top){
        bv[0][1]=rect1.miny;
        bv[1][1]=rect2.maxy;
    } else {
        bv[0][1]=rect1.maxy;
        bv[1][1]=rect2.miny;
    }
    if (right){
        bv[0][0]=rect1.minx;
        bv[1][0]=rect2.maxx;
    }else {
        bv[0][0]=rect1.maxx;
        bv[1][0]=rect2.minx;
    }
    return (pow(pow(bv[1][0]-bv[0][0],2.f) + pow(bv[1][1]-bv[0][1],2.f),.5f));
}

template <typename valType> Rectangle2<valType>
Rectangle2<valType>::EMPTY(Rectangle2<valType>::MAX_VAL, Rectangle2<valType>::MAX_VAL,
                           Rectangle2<valType>::MIN_VAL, Rectangle2<valType>::MIN_VAL);

typedef Rectangle2<double> Rectangle2d;
typedef Rectangle2<int> Rectangle2i;
typedef FullRectangle2<int> FullRectangle2i;

typedef bool(*Rectangle2iComparator) (const Rectangle2i &lhs,const Rectangle2i &rhs);
typedef set<Rectangle2i, Rectangle2iComparator> Rectangles2I;

typedef set<FullRectangle2i, Rectangle2iComparator> FullRectangles2I;

typedef bool(*Rectangle2dComparator) (const Rectangle2d &lhs, const Rectangle2d &rhs);
typedef set<Rectangle2d, Rectangle2dComparator> Rectangles2D;


#endif
