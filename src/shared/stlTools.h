
#ifndef CPP_STLTOOLS_H
#define CPP_STLTOOLS_H

#ifdef _WINDOWS
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>

void usleep(__int64 usec);
void sleep(int usec);
#endif

#ifdef _LINUX
#define STRCPY(X,Y) strcpy(X,Y)
#else
#define STRCPY(X,Y) std::strcpy(X,Y)
#endif

#ifdef _WINDOWS
typedef LONG64 dplong;
typedef ULONG64 udplong;
#else
#ifdef _RASPBERRY_PI
typedef long long dplong;
typedef unsigned long long udplong;
#else
typedef long dplong;
typedef unsigned long udplong;
#endif
#endif

//for windows
#define NOMINMAX

#include <list>
#include <string>
#include <algorithm>
#include <vector>
#include <iterator>
#include <iostream>
#include <map>
#include <set>
#include <stack>
#ifndef NO_DP
#include "primitive.h"
#endif
// string.h for memcpy
#include <string.h>

#ifdef _LINUX
#include <functional>
#include <cctype>
#include <cfloat>
#define MAXFLOAT FLT_MAX
#include <climits>
#endif

#ifdef _WINDOWS
#include <functional>
#include <cctype>
#include <cfloat>
#define MAXFLOAT FLT_MAX
#else
#include <unistd.h>
#endif

using namespace std;

int cwrite(int fildes, const void *buf, size_t nbyte);

string GetToken(const string &str, int tok);

void Tokenize(const string& str, vector<string>& tokens, const string& delimiters = " ", bool returnDelims=false, bool noWhitespace = true);

//template<typename T> void printVector(vector<T> &vec, ostream& out);
//template<typename T> void printVectorPtr(vector<T> &vec, ostream& out);
template<typename T>
void printVector(vector<T> &vec, ostream& out, string quote="", string delimiter="\t"){
  typename vector<T>::const_iterator it;
  for (it=vec.begin();it !=vec.end();){
    out << quote << *it << quote ;
    it++;
    if (it!=vec.end()){
      out << delimiter;
    }
  }
  out << endl;
};

template<typename T>
void printVectorPtr(vector<T> &vec, ostream& out, bool endline=true, bool brackets=false, bool comma=true){
  typename vector<T>::const_iterator it;
  if (brackets){
    out << "[ " ;
  }
  for (it=vec.begin();it !=vec.end();){
    if (*it){
      out << **it ;
    } else {
      out << "NULL" ;
    }
    it++;
    if (it!=vec.end()){
      if (comma){
	out << "\t";
      } else {
	out << ", ";
      }
    }
  }
  if (brackets){
    out << " ]" ;
  }
  if (endline){
    out << endl;
  }
};

template<typename T>
void printListPtr(list<T> &vec, ostream& out, bool endline=true, bool brackets=false, bool comma=true){
  typename list<T>::const_iterator it;
  if (brackets){
    out << "[ " ;
  }
  for (it=vec.begin();it !=vec.end();){
    if (*it){
      out << **it ;
    } else {
      out << "NULL" ;
    }
    it++;
    if (it!=vec.end()){
      if (comma){
	out << "\t";
      } else {
	out << ", ";
      }
    }
  }
  if (brackets){
    out << " ]" ;
  }
  if (endline){
    out << endl;
  }
};

template<typename T>
void printSet(set<T> &hashset, ostream& out){
  out << "[ ";
  for (typename set<T>::iterator it=hashset.begin();it!=hashset.end();){  
    out << *it ;
    it++;
    if (it!=hashset.end()){
      out << ", ";
    }
  }
  out << " ]";
}

template<typename T,typename M>
void push_back_all(vector<T> &list, vector<M> &addList){
  for (typename vector<M>::iterator it=addList.begin();it!=addList.end();){
    list.push_back((T)*it);
    it++;
  }
  return;
};

template<typename T,typename M>
void insert_all(set<T> &list, vector<M> &addList){
    for (auto it=addList.begin();it!=addList.end();){
        list.insert((T)*it);
        it++;
    }
    return;
};

template<typename T,typename M>
void push_back_all(vector<T> &list, set<M> &addList){
    for (typename set<M>::iterator it=addList.begin();it!=addList.end();){
        list.push_back((T)*it);
        it++;
    }
    return;
};

string getFileAsString(const string &filename); 

void trimString(string& str, const string chrs=" \t\n");
void trimStrings(vector<string>& str, const string chrs=" \t\n");
bool isWhiteSpace(string& str);
/* hasAndPerSign(): returns true if "&" exists without a backslash '\' before it */
extern string andPerSign;
bool hasAndPerSign(string& str);

//Object *createObject(void *ptr);

const string BoolToString(bool num);
const string IntToString(int num);
const string LongToString(dplong num);
const string DoubleToString(double num);
const string FloatToString(float num);

double StringToDouble(const string str, const bool quiet=false);
int StringToHex(const string str);
int StringToInt(const string str);
dplong StringToLong(const string str);
bool StringToBool(const string str);
string StringToString(const string str);

bool StringisInt(const string str);
bool StringisDouble(const string str);
bool StringisNumber(const string str);

string toLower(string str);

string replaceAllWith(string orig, string cmpStr, string rplStr);

bool isNaN(double val);
bool isInfinity(double val);
bool isNegInfinity(double val);
bool isPosInfinity(double val);

extern double TOLER;
extern double PLANE_SPLIT;
extern int NEGATIVE;
extern int ZERO;
extern int POSITIVE;

bool doubleIsEqual(double , double);
int computeSign(double tmp);

template<typename T, size_t N>
T * vend(T (&ra)[N]) {
    return ra + N;
}

#ifndef NO_DP
void writeBoolToSocket(int, bool);
void writeByteToSocket(int, unsigned char);
void writeIntToSocket(int, int);
void writeBoolToMem(void *mem, bool v);
void writeShortToMem(void *mem, short v);
void writeIntToMem(void *mem, int v);
void *writeStringToMem(void *mem, string &str);
void *writeByteArrayToMem(void *mem, void *src, int srclen);
int readBoolFromSocket(int, bool &);
int readByteFromSocket(int, unsigned char &);
int readIntFromSocket(int, int &);
int readStringFromSocket(int, string &retstr);
int readIntFromPtr(void *,int offset=0);
void swapdata(void *, int);

class ReadBuffer {
private:
    unsigned char *dataRead;
    int dataReadPlace, dataReadTotalLength;
    bool releaseOnDelete;
public:
    ReadBuffer(int sock, int size);
    ReadBuffer(void *arr, int size, bool releaseOnDelete=true);
    virtual ~ReadBuffer();
    int readInt(int &val);
    int readString(string &retstr);
    void readStringWithLength(int len, string &retstr);
    int readStringWithDelim(string &retstr, const char *delimiter, const bool includeDelimiter = false);
    int readBool(bool &val);
    bool readBytesDirect(int nbytes, void **bytes);
    int readBytes(int nbytes, void *bytes);
    bool isAtEnd(){ return dataReadPlace >= dataReadTotalLength; };
    int getReadPlace(){ return dataReadPlace; };
    bool computeAndCheckCheckSum();
    bool computeAndWriteCheckSum();
    int skip(int);
    /*
    int readByte(unsigned char &);
    int readInt(int &val);
    int readString(string &retstr);
    int readStringVector(vector<string> &retstr);
    int readIntVector(vector<int> &retstr);
    */
};
#endif

#ifndef NO_DP
bool computeAndCheckSumIn(bool check, bool write, unsigned char *dataRead, int dataReadTotalLength);

class WriteBuffer {
protected:
    vector< pair<int, void*> > *dataToWrite;
    int dataToWriteTotalLength;
public:
    WriteBuffer();
    virtual ~WriteBuffer();
    void *getAllData(int &);
    void writeShort(short val);
    void writeInt(int val);
    void writeLong(unsigned long val);
    void writeBool(bool val);
    void writeString(string &retstr);
    void writeData(void *data, int dataLen);
    void writeStringVector(vector<string> &retstr);
    void writeIntVector(vector<int> &retstr);
    void *getDataItem(int itemnum);
    int getLength() { return dataToWriteTotalLength; }
};
#endif

#ifndef NO_DP
class SocketBuffer : public WriteBuffer {
private:
    int sockfd;
public:
    SocketBuffer(int);
    virtual ~SocketBuffer();
    int write();
    void computeAndWriteCheckSum();
    void setSocket(int s){sockfd=s; }
};
#endif

string getHostName();

long getMilliCount();
void *Base64Decode(string &stringobj, int &len);

float frand();
string genRandomString(int len=5);
int genRandomInt();

template<typename T1,typename T2,typename T3> class triple {
 public:
 triple(const T1 &t1,const T2 &t2,const T3 &t3) : first(t1), second(t2), third(t3){ }
 triple(const triple &arg) : first(arg.first), second(arg.second), third(arg.third) { }
 triple(){}
 triple &operator=(const triple& other) {
    //Check for self-assignment
    if (this == &other)
        return *this;
    first = other.first;
    second = other.second;
    third = other.third;
    return *this;
 }
  T1 first;
  T2 second;
  T3 third;
};

template<typename T1,typename T2,typename T3,typename T4> class quad {
 public:
 quad(const quad<T1, T2, T3, T4> &qarg) : first(qarg.first), second(qarg.second), third(qarg.third), fourth(qarg.fourth){
 }
 quad(const T1 &t1,const T2 &t2,const T3 &t3,const T4 &t4) : first(t1), second(t2), third(t3), fourth(t4){
  }
  T1 first;
  T2 second;
  T3 third;
  T4 fourth;
};

template<typename T1,typename T2,typename T3,typename T4,typename T5> class quintuple {
 public:
 quintuple(const T1 &t1,const T2 &t2,const T3 &t3,const T4 &t4,const T5 &t5) : first(t1), second(t2), third(t3), fourth(t4), fifth(t5){
  }
  T1 first;
  T2 second;
  T3 third;
  T4 fourth;
  T5 fifth;
};

string getLocalIPAddress();
string getLocalIPBroadcastAddress();
string getLocalIP6Address();
string getLocalIP6Interface();

// trim from start
static inline std::string &ltrim(std::string &s) {
    s.erase(s.begin(), std::find_if(s.begin(), s.end(), std::not1(std::ptr_fun<int, int>(std::isspace))));
    return s;
}

// trim from end
static inline std::string &rtrim(std::string &s) {
    s.erase(std::find_if(s.rbegin(), s.rend(), std::not1(std::ptr_fun<int, int>(std::isspace))).base(), s.end());
    return s;
}

// trim from both ends
static inline std::string &trim(std::string &s) {
    return ltrim(rtrim(s));
}

static inline bool isAllWhiteSpace(const std::string &s){
    return s.find_first_not_of(" \t\n\r") == std::string::npos;
}

// first can be +-INFINITY
double dpmin(double first, double second, bool ret[]);
double dpmax(double first, double second, bool ret[]);

template <typename T>
ostream& operator <<(ostream &os, const vector<T> &list){
    auto it = list.begin();
    bool cont = it!=list.end();
    while (cont){
        auto obj = (*it);
        it++;
        cont = it!=list.end();
        os << obj;
        if (cont)
            os << ", ";
    }
    return os;
};

template <typename T>
ostream& operator <<(ostream &os, const set<T> &list){
    auto it = list.begin();
    bool cont = it!=list.end();
    while (cont){
        auto obj = (*it);
        it++;
        cont = it!=list.end();
        os << obj;
        if (cont)
            os << ", ";
    }
    return os;
};

template <typename T, typename T2>
ostream& operator <<(ostream &os, const set<T,T2> &list){
    auto it = list.begin();
    bool cont = it!=list.end();
    while (cont){
        auto obj = (*it);
        it++;
        cont = it!=list.end();
        os << obj;
        if (cont)
            os << ", ";
    }
    return os;
};

void *convertStringsToContiguousNullDelimitedArray(const vector<string> &lstr);

const string urlencode(const string);

#ifdef _WINDOWS
#undef min
#undef max
#endif

#endif
