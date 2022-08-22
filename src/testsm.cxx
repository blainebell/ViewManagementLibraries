
#include "IntervalTree.h"
#include "DataListInterface.h"

int main() {
  IntervalTree2i intervalTree;
  intervalTree.add(Rectangle2i(0,0,640,480));
  FullRectangle2i fs(1, 200, 200, 400, 400);
  intervalTree.addFullRectangleInEmptySpace(fs, &intervalTree);
  cout << "intervalTree: " << intervalTree << endl;
  auto allRects = intervalTree.getAll();
  cout << "allRects: " << endl;
  for (auto & element : *allRects) {
    cout << "   " << element << endl;
  }
  delete allRects;
  return 0;
}
