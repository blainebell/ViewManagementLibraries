
#include "stlTools.h"
#include "Rectangle2.h"
#include <cmath>
#include <climits>

template <> double Rectangle2<double>::MAX_VAL = MAXFLOAT;
template <> double Rectangle2<double>::MIN_VAL = -MAXFLOAT;
template <> int Rectangle2<int>::MAX_VAL = INT_MAX;
template <> int Rectangle2<int>::MIN_VAL = INT_MIN;

template <> Rectangle2iComparator Rectangle2<int>::default_compare = &Rectangle2<int>::minx_compare;
template <> Rectangle2dComparator Rectangle2<double>::default_compare = &Rectangle2<double>::minx_compare;


template <> int Rectangle2<int>::type_compare(const int &a, const int &b){
    if (a==b){
        return 0;
    } else if (a < b){
        return -1;
    } else {
        return 1;
    }
}

template <> int Rectangle2<double>::type_compare(const double &a, const double &b){
    if (doubleIsEqual(a,b)){
        return 0;
    } else if (a < b){
        return -1;
    } else {
        return 1;
    }
}


template <> bool Rectangle2<int>::type_equals(const int &a, const int &b){
    return (a==b);
}

template <> bool Rectangle2<double>::type_equals(const double &a, const double &b){
    return (doubleIsEqual(a,b));
}
