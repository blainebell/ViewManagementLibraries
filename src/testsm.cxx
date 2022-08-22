
#include "IntervalTree.h"
#include "DataListInterface.h"

int main() {
  IntervalTree2i intervalTree;
  intervalTree.add(Rectangle2i(0,0,640,480));
  FullRectangle2i fs(1, 200, 200, 400, 400);
  intervalTree.addFullRectangleInEmptySpace(fs, &intervalTree);
  cout << "intervalTree: " << intervalTree << endl;
  return 0;
}
