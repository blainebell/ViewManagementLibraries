
#include "IntervalTree.h"

template <> IntervalDimension<int, Rectangle2<int> > IntervalTreeDefaults<int, Rectangle2<int>>::default_interval_dimension(1, &Rectangle2i::minx_get, &Rectangle2i::maxx_get);
template <> IntervalDimension<int, Rectangle2<int> > IntervalTreeDefaults<int, Rectangle2<int>>::secondary_interval_dimension(2, &Rectangle2i::miny_get, &Rectangle2i::maxy_get);

template <> IntervalDimension<int, FullRectangle2<int> > IntervalTreeDefaults<int, FullRectangle2<int>>::default_interval_dimension(1, &FullRectangle2i::minx_get, &FullRectangle2i::maxx_get);
template <> IntervalDimension<int, FullRectangle2<int> > IntervalTreeDefaults<int, FullRectangle2<int>>::secondary_interval_dimension(2, &FullRectangle2i::miny_get, &FullRectangle2i::maxy_get);



template <> IntervalDimension<double, Rectangle2<double> > IntervalTreeDefaults<double, Rectangle2<double>>::default_interval_dimension(1, &Rectangle2d::minx_get, &Rectangle2d::maxx_get);
template <> IntervalDimension<double, Rectangle2<double> > IntervalTreeDefaults<double, Rectangle2<double>>::secondary_interval_dimension(2, &Rectangle2d::miny_get, &Rectangle2d::maxy_get);


template <> IntervalDimension<double, Rectangle3<double> > IntervalTreeDefaults<double, Rectangle3<double>>::default_interval_dimension(1, &Rectangle3d::minx_get, &Rectangle3d::maxx_get);
template <> IntervalDimension<double, Rectangle3<double> > IntervalTreeDefaults<double, Rectangle3<double>>::secondary_interval_dimension(2, &Rectangle3d::miny_get, &Rectangle3d::maxy_get);
template <> IntervalDimension<double, Rectangle3<double> > IntervalTreeDefaults<double, Rectangle3<double> >::tertiary_interval_dimension(3, &Rectangle3d::minz_get, &Rectangle3d::maxz_get);


template <> IntervalDimension<double, FullRectangle3d > IntervalTreeDefaults<double, FullRectangle3d >::default_interval_dimension(1, &FullRectangle3d::minx_get, &FullRectangle3d::maxx_get);
template <> IntervalDimension<double, FullRectangle3d > IntervalTreeDefaults<double, FullRectangle3d>::secondary_interval_dimension(2, &FullRectangle3d::miny_get, &FullRectangle3d::maxy_get);
template <> IntervalDimension<double, FullRectangle3d > IntervalTreeDefaults<double, FullRectangle3d >::tertiary_interval_dimension(3, &FullRectangle3d::minz_get, &FullRectangle3d::maxz_get);
