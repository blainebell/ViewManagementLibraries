

#include "Rectangle3.h"
#include "stlTools.h"
#include <cmath>


//Rectangle3<valType>::EMPTY();


//template <> Rectangle3<double> Rectangle3<double>::EMPTY(MAXFLOAT, MAXFLOAT, MAXFLOAT, MAXFLOAT, MAXFLOAT, MAXFLOAT);


template <> Rectangle3iComparator Rectangle3<int>::default_compare = &Rectangle3<int>::minx_compare;
template <> Rectangle3dComparator Rectangle3<double>::default_compare = &Rectangle3<double>::minx_compare;
