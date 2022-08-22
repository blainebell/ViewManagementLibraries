

#include "stlTools.h"
#include <sstream>
#include <cstdlib>
#include <math.h>
#ifndef _WINDOWS
#include <unistd.h>
#endif
#include <cmath>
#include <cfloat>

#include <stdlib.h>

extern void dplog(string);
#ifdef _WINDOWS
#include <io.h>
/*int __cdecl read(int fh, void *dest, unsigned int sz) {
  return _read(fh, dest, sz);
}
int __cdecl write(int fh, void *buf, unsigned int sz) {
  return _write(fh, buf, sz);
}*/
#else
#include <sys/uio.h>

#include <sys/timeb.h>

#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netdb.h>
#include <ifaddrs.h>
#include <unistd.h>
#endif

#ifdef _LINUX
#define ISINF(x) isinf(x)
#else
#define ISINF(x) ::isinf(x)
#endif

string GetToken(const string &line, int tok){
  size_t prev = 0, pos = 0;
  do {
    pos = line.find_first_of(" ");
    if (pos == string::npos)
      return "";
    if (tok==0)
      return line.substr(prev, pos);
    prev = pos;
    tok--;
  } while (tok > 0);
  return "";
}

void Tokenize(const string& str, vector<string>& tokens, const string& delimiters, bool returnDelims, bool noWhitespace){
    // Skip delimiters at beginning.
    if (str.length()==0){
        return;
    }
    string::size_type lastPos = str.find_first_not_of(delimiters, 0);
    if (returnDelims){
        for (int i=0; i<lastPos; i++){
            tokens.push_back(str.substr(i, 1));
        }
    }
    // Find first "non-delimiter".
    string::size_type pos     = str.find_first_of(delimiters, lastPos);
    
    while (string::npos != pos || string::npos != lastPos){
        // Found a token, add it to the vector.
        string tstr = str.substr(lastPos, pos - lastPos);
        if (!noWhitespace || !isWhiteSpace(tstr))
            tokens.push_back(tstr);
        // Skip delimiters.  Note the "not_of"
        lastPos = str.find_first_not_of(delimiters, pos);
        if (lastPos != string::npos){
            if (returnDelims){
                for (auto i=pos; i<lastPos; i++){
                    string sstr = str.substr(i, 1);
                    if (!noWhitespace || !isWhiteSpace(sstr))
                        tokens.push_back(str.substr(i, 1));
                }
            } else if (!noWhitespace){
                for (auto i=pos+1; i<lastPos; i++){
                    tokens.push_back("");
                }
            }
        }
        // Find next "non-delimiter"
        pos = str.find_first_of(delimiters, lastPos);
    }    
}

#include <fstream>
//#include <sys/stat.h>

#if defined(WIN32) || defined(WIN64)
// Copied from linux libc sys/stat.h:
#define S_ISREG(m) (((m) & S_IFMT) == S_IFREG)
#define S_ISDIR(m) (((m) & S_IFMT) == S_IFDIR)
#endif
string getFileAsString(const string &filename){
  ifstream t(filename);
  string retstr;
  if (!t.fail()){
    t.seekg(0, ios::end);
    retstr.reserve(t.tellg());
    t.seekg(0, ios::beg);
    
    retstr.assign((istreambuf_iterator<char>(t)),
		  istreambuf_iterator<char>());
  }
  return retstr;
}

void trimStrings(vector<string>& str, const string chrs){
    for (auto it=str.begin(); it!=str.end(); it++){
        trimString(*it);
    }
}

void trimString(string& str, const string chrs){
  // Trim Both leading and trailing spaces
  // Find the first character position after excluding leading blank spaces
  int startpos = (int)str.find_first_not_of(chrs);
  // Find the first character position from reverse af
  int endpos = (int)str.find_last_not_of(chrs);
  // if all spaces or empty return an empty string
  if(( string::npos == startpos ) || ( string::npos == endpos)){
    str = "";
  } else {
    str = str.substr( startpos, endpos-startpos+1 );
  }
}

bool isWhiteSpace(string& str){
  // Trim Both leading and trailing spaces
  // Find the first character position after excluding leading blank spaces
  int startpos = (int)str.find_first_not_of(" \t\n");
//    cout << "isWhiteSpace: str='" << str << "' returns " << (string::npos == startpos) << endl;
  return string::npos == startpos;
}

/* hasAndPerSign(): returns true if "&" exists without a backslash '\' before it */
string andPerSign = "&";
bool hasAndPerSign(string& str){
  std::size_t found = str.find("&");
  if (found==string::npos){
    return (false);
  }
  int pl = (int)found;
  while (pl>=0){
    if (pl>0 && str.at(pl-1)=='\\'){
      pl++;
      pl = (int)str.find("&", pl);
    } else {
      return (true);
    }
  }
  return (false);
}

//Object *createObject(void *ptr);

const string BoolToString(bool val) {
  if (val){
    return ("True");
  } else {
    return ("False");
  }
}
const string IntToString(int num){
  stringstream ss;
  ss << num;
  return ss.str();
}
const string LongToString(dplong num){
  stringstream ss;
  ss << num;
  return ss.str();
}
const string DoubleToString(double num){
  stringstream ss;
  ss << fixed << num;
  return ss.str();
}
const string FloatToString(float num){
  stringstream ss;
  ss << fixed << num;
  return ss.str();
}

int StringToHex(const string str){
  int sz = (int)str.size();
  int pl, a0 = 1, ret = 0;
  char ch;
  unsigned int a1;
  for (pl = sz-1; pl>=0; pl--){
    ch = str.at(pl);
    if (ch >= '0' && ch <= '9'){
      a1 = ch - '0';
    }
    if (ch >= 'a' && ch <= 'f'){
      a1 = ch - 'a' + 10;
    }
    ret += a1 * a0;
    a0 *= 16;
  }
  return ret;
}

string StringToString(const string str){
    string strret = str;
    return strret;
}

int StringToInt(const string str){
  string strc ;
  size_t found;
  strc =  toLower(str);
  if ((found=strc.find("x"))!=string::npos){
    return(StringToHex(strc.substr(found+1)));
  } else if ((found=strc.find("inf"))!=string::npos){
      if (strc.find("-")==0){
          return -INT32_MAX;
      } else {
          return INT32_MAX;
      }
  }

  int reti = atoi(str.c_str());
  return reti;
}

dplong StringToLong(const string str){
  string strc ;
  size_t found;
  strc =  toLower(str);
  if ((found=strc.find("x"))!=string::npos){
    return(StringToHex(strc.substr(found+1)));
  }
#if defined(IOS) || defined(_WINDOWS) || defined(_RASPBERRY_PI)
  dplong retl = atoll(str.c_str());
#else
  dplong retl = atol(str.c_str());
#endif
  return retl;
}

bool is_possible_number(const std::string &str);

bool is_possible_number(const std::string &str)
{
    return str.find_first_not_of("0123456789.-eE") == std::string::npos;
}

string toLower(string str){
    string rstr(str);
    for (int i=0; i< rstr.size(); i++){
        rstr[i] = (char)tolower((int)str[i]);
    }
    return rstr;
}

double StringToDouble(const string str, const bool quiet){
  if (is_possible_number(str)){
    return atof(str.c_str());
  } else {
    size_t found;
    string lstr = toLower(str);
    if ((found=lstr.find("inf"))!=string::npos){
    if (lstr.find("-")==0){
        return -INFINITY;
      } else {
        return INFINITY;
      }
    }
  }
  if (!quiet){
    stringstream ss;
    ss << "WARNING: StringToDouble: cannot convert str='" << str << "'" << endl;
    cerr << ss.str() << endl;
  }
  return 0.0;
}

bool StringToBool(const string str){
  string strc;
  strc =  toLower(str);
  if (strc == "true" || strc == "1" || strc == "t"){
    return true;
  }
  return false;
}

bool StringisInt(const string str){
  int i = StringToInt(str);
  return (IntToString(i)==str);
}
bool StringisDouble(const string str){
  /* TODO : Probably need to take into account decimal places */
  double d = StringToDouble(str, true);
  return (DoubleToString(d)==str);
}
bool StringisNumber(const string str){
  return(StringisInt(str) || StringisDouble(str));
}
string replaceAllWith(string orig, string cmpStr, string rplStr){
  string string1(orig);
  auto cmpStrLen = cmpStr.length();
  auto rplStrLen = rplStr.length();
  auto position = string1.find( cmpStr ); // find first
   while ( position != string::npos ) {
     string1.replace( position, cmpStrLen, rplStr );
     position = string1.find( cmpStr, position + rplStrLen );
   }
   return string1;
}

bool isNaN(double val){
  return (val != val);
}

bool isInfinity(double val){
    return ISINF(val);
}

bool isNegInfinity(double val){
    return ISINF(val) && (-INFINITY == val);
}

bool isPosInfinity(double val){
    return ISINF(val) && (INFINITY == val);
}

double TOLER=0.00000763;
double PLANE_SPLIT=(2*TOLER);
int NEGATIVE=-1;
int ZERO=0;
int POSITIVE=1;

int computeSign(double tmp){
    if (tmp < -TOLER){
        return (-1);
    } else if (tmp>TOLER){
        return (1);
    } else {
        return (0);
    }
}

bool doubleIsEqual(double val1, double val2){
  if (isNaN(val1) != isNaN(val2)){
    return false;
  } else {
    return (!(fabs(val2-val1)>TOLER));
  }
}

#ifndef NO_DP
void writeBoolToSocket(int s, bool b){
  cwrite(s, &b, sizeof(bool));
}
void writeByteToSocket(int s, unsigned char b){
  cwrite(s, &b, sizeof(unsigned char));
}
void writeIntToSocket(int s, int v){
  unsigned char out[4];
  out[0] = ((v >> 24) & 0xFF);
  out[1] = ((v >> 16) & 0xFF);
  out[2] = ((v >> 8) & 0xFF);
  out[3] = (v & 0xFF);
  cwrite(s, out, 4);
  //  write(s, &v, sizeof(int));
}

void writeBoolToMem(void *mem, bool v){
    unsigned char *out = (unsigned char *)mem;
    out[0] = v ? 1 : 0;
}

void writeShortToMem(void *mem, short v){
    unsigned char *out = (unsigned char *)mem;
    out[0] = ((v >> 8) & 0xFF);
    out[1] = (v & 0xFF);
}

void writeIntToMem(void *mem, int v){
  unsigned char *out = (unsigned char *)mem;
  out[0] = ((v >> 24) & 0xFF);
  out[1] = ((v >> 16) & 0xFF);
  out[2] = ((v >> 8) & 0xFF);
  out[3] = (v & 0xFF);
}
void *writeStringToMem(void *mem, string &str){
  int len = (int)str.length();
  writeIntToMem(mem, len);
  memcpy(((char*)mem) + 4, str.c_str(), len);
  return ((char*)mem) + 4 + len;
}
void *writeByteArrayToMem(void *mem, void *src, int srclen){
  writeIntToMem(mem, srclen);
  memcpy(((char*)mem) + 4, src, srclen);
  return ((char*)mem) + 4 + srclen;
}

int readIntFromPtr(void *ptr, int offset){
    unsigned char *rval = ((unsigned char *)ptr)+offset;
    int ival = ((rval[0] << 24) + (rval[1] << 16) + (rval[2] << 8) + rval[3]);
    return ival;
}

int readIntFromSocket(int sockfd, int &val){
  unsigned char rval[4];
  int tot = 0, ret;
  while (tot<4){
#ifdef _WINDOWS
	  ret = recv(sockfd, (char*)&rval[tot], 4 - tot, 0);
#else
	  ret = read(sockfd, &rval[tot], 4 - tot);
#endif
      if (ret <= 0){
          cerr << "readIntFromSocket: ERROR ret=" << ret << " tot=" << tot << endl;
          return tot;
      } else if ((tot + ret) < 4){
          cout << "readIntFromSocket: read only " << ret << " bytes, continuing" << endl;
      }
      tot += ret;
  }
  int ival = ((rval[0] << 24) + (rval[1] << 16) + (rval[2] << 8) + rval[3]);
  val = ival;
  return ret;
}
int readBoolFromSocket(int sockfd, bool &val){
  bool retch;
#ifdef _WINDOWS
  int ret = recv(sockfd, (char*)&retch, 1, 0);
#else
  int ret = read(sockfd, &retch, 1);
#endif
  if (ret)
    val = (bool)retch;
  return ret;
}

int readByteFromSocket(int sockfd, unsigned char &retch){
#ifdef _WINDOWS
	return recv(sockfd, (char*)&retch, sizeof(unsigned char), 0);
#else
	return read(sockfd, &retch, sizeof(unsigned char));
#endif
}

int readStringFromSocket(int sockfd, string &retstr){
  int nch, n;
  n = readIntFromSocket(sockfd, nch);
  if (n){
    char *ch = new char[nch];
#ifdef _WINDOWS
    n = recv(sockfd, ch, nch, 0);
#else
	n = read(sockfd, ch, nch);
#endif
    if (n<0){
        cout << "WARNING: readStringFromSocket n=" << n << endl;
        return 0;
    } else if (n==nch){
      retstr = string(ch, nch);
      return true;
    } else {
      if (n >0 && n < nch) {
	int totalreadin = n, totalneeded = nch - n;
	bool cont = true;
	//	cout << "  readStringFromSocket: read in only n=" << n << " out of nch=" << nch << ", need to read in totalneeded=" << totalneeded << " more" << endl;
	while (totalneeded > 0 && cont){
	  char *newch = ch + totalreadin;
#ifdef _WINDOWS
	  n = recv(sockfd, newch, totalneeded, 0);
#else
	  n = read(sockfd, newch, totalneeded);
#endif
	  if (n>0){
	    totalreadin += n;
	    totalneeded -= n;
	    //	    cout << "  readStringFromSocket: read in n=" << n << " more characters, totalling " << (totalreadin) << " so far, still need " << (totalneeded) << endl;
	  } else {
	    cont = false;
	  }
	}
	if (totalneeded==0){
	  //	  cout << "  readStringFromSocket: returning string with nch=" << nch << endl;
	  retstr = string(ch, nch);
	  return true;
	}
      } else {
	cout << "WARNING: readStringFromSocket read in string with incorrect number of characters n=" << n << " nch=" << nch << endl;
      }
    }
    delete ch;
  } else {
    retstr = "";
    //    cout << "WARNING: readStringFromSocket did not read number of characters" << endl;
  }
  return 0;
}
#endif
void swapdata(void *ptrARG, int sz){
  char *ptr = (char*)ptrARG;
  char *endptr = ptr + sz - 1;
  char tmp;
  while (ptr < endptr){
    tmp = *endptr;
    *endptr = *ptr;
    *ptr = tmp;
    ptr++;
    endptr--;
  }
}

#ifndef NO_DP
WriteBuffer::WriteBuffer() : dataToWrite(0), dataToWriteTotalLength(0) {
}
#endif

#ifndef NO_DP
SocketBuffer::SocketBuffer(int fd) : sockfd(fd), WriteBuffer() {
}
#endif

#ifndef NO_DP
bool ReadBuffer::computeAndCheckCheckSum(){
  if (dataReadPlace != 0){
    // check checksum needs to be at the beginning of ReadBuffer
    return false;
  }
  return computeAndCheckSumIn(true, false, dataRead, dataReadTotalLength);
}
bool ReadBuffer::computeAndWriteCheckSum(){
  if (dataReadPlace != 0){
    // check checksum needs to be at the beginning of ReadBuffer
    return false;
  }
  return computeAndCheckSumIn(false, true, dataRead, dataReadTotalLength);
}

bool computeAndCheckSumIn(bool check, bool write, unsigned char *dataRead, int dataReadTotalLength){
    unsigned char *checksum = dataRead;
    int padding = dataReadTotalLength % 2;
    int i = 2;
    udplong sum = (udplong)0l;
    for (;i < dataReadTotalLength - padding;  i+=2){
        sum += ( ( ( dataRead[i]) << 8 ) & 0xFF00 ) + ( ( dataRead[i+1] ) & 0xFF );
    }
    if (padding){
        sum += ( ( ( dataRead[i]) << 8 ) & 0xFF00 );
    }
    while (sum>>16)
        sum = (sum & 0xFFFF)+(sum >> 16);

    sum = ~sum;
    if (check){
      if ( ( ( (sum & 0xff00) >> 8) == (int)(checksum[0] & 0xff) ) &&
	   ( (sum & 0xff) == (int)(checksum[1] & 0xff) ) ){
        return true;
      }
      return false;
    }
    if (write){
      checksum[0] = ((sum & 0xff00) >> 8);
      checksum[1] = (sum & 0xff);
      return true;
    }
    return false;
}
#endif

#ifndef NO_DP
void SocketBuffer::computeAndWriteCheckSum(){
    vector< pair<int, void*> >::iterator it=dataToWrite->begin();
    if (it->first != 2){  // first data block needs to be checksum, i.e., 2 bytes
        cerr << "SocketBuffer::computeAndWriteCheckSum: WARNING: first block is not 2 bytes, checksum cannot be written" << endl;
    }
    unsigned char *checksum = (unsigned char *)it->second;
    int padding = dataToWriteTotalLength % 2;
    udplong sum = (udplong)0l;
    unsigned char *lastbyte = 0;
    it++;  // skip checksum for calculation
    for (; it!=dataToWrite->end(); it++){
        int len = it->first, start = 0;
        unsigned char *data = (unsigned char *)it->second;
        if (lastbyte){
            sum += ( ( ( (int)lastbyte[0]) << 8 ) & 0xFF00 ) + ( ( (int)data[0] ) & 0xFF );
            start = 1;
            lastbyte = 0;
        }
        padding = (len - start) % 2;
        for (int i = start; i < len - padding; i+=2){
            sum += ( ( ( (int)data[i]) << 8 ) & 0xFF00 ) + ( ( (int)data[i+1] ) & 0xFF );
        }
        if (padding){
            lastbyte = ((unsigned char *)it->second) + len - 1;
        }
    }
    if (lastbyte){
        sum += ( ( (int)lastbyte[0]) << 8 ) & 0xFF00;
    }
    while (sum>>16){
        sum = (sum & 0xFFFF)+(sum >> 16);
    }
    sum = ~sum;
//    cout << "SocketBuffer::computeAndWriteCheckSum: checksum=" << (sum & 0xFFFF) << " bytes: " << (sum & 0xff00) << " " << (sum & 0xff) << endl;
    checksum[0] = (sum & 0xff00) >> 8;
    checksum[1] = sum & 0xff;
}
#endif

#ifndef NO_DP
void *WriteBuffer::getAllData(int &ad){
  vector< pair<int, void*> > *dataToWriteTmp = dataToWrite;
  void *allData, *tmpPtr ;
  int allDataLength = dataToWriteTotalLength;
  dataToWrite = NULL;
  dataToWriteTotalLength = 0;
  allData = malloc(allDataLength);
  tmpPtr = allData;
  for (vector< pair<int, void*> >::iterator it=dataToWriteTmp->begin(); it!=dataToWriteTmp->end(); it++){
    int len = it->first;
    void *data = it->second;
    memcpy(tmpPtr, data, len);
    free(data);
    tmpPtr = ((unsigned char*)tmpPtr) + len;
  }
  ad = allDataLength;
  delete dataToWriteTmp;
  return allData;
}
#endif

#ifndef NO_DP
int SocketBuffer::write(){
  vector< pair<int, void*> > *dataToWriteTmp = dataToWrite;
  void *allData, *tmpPtr ;
  int allDataLength = dataToWriteTotalLength;
  dataToWrite = NULL;
  dataToWriteTotalLength = 0;
  if (!dataToWriteTmp || !allDataLength)
      return 0;
  allData = malloc(allDataLength);
  tmpPtr = allData;
  for (vector< pair<int, void*> >::iterator it=dataToWriteTmp->begin(); it!=dataToWriteTmp->end(); it++){
    int len = it->first;
    void *data = it->second;
    memcpy(tmpPtr, data, len);
    free(data);
    tmpPtr = ((unsigned char*)tmpPtr) + len;
  }
  //  cout << "SocketBuffer::write(): writing allDataLength=" << allDataLength << endl;
  int written = cwrite(sockfd, allData, allDataLength);

  free(allData);
  delete dataToWriteTmp;
  return written;
}
#endif

#ifndef NO_DP
void WriteBuffer::writeShort(short val){
    unsigned char *out = (unsigned char *)malloc(2);
    if (!dataToWrite) dataToWrite = new vector< pair<int, void*> >();
    out[0] = ((val >> 8) & 0xFF);
    out[1] = (val & 0xFF);
    dataToWrite->push_back( pair<int, void*>(2, out) );
    dataToWriteTotalLength += 2;
}

void WriteBuffer::writeInt(int val){
  unsigned char *out = (unsigned char *)malloc(4);
  if (!dataToWrite) dataToWrite = new vector< pair<int, void*> >();
  out[0] = ((val >> 24) & 0xFF);
  out[1] = ((val >> 16) & 0xFF);
  out[2] = ((val >> 8) & 0xFF);
  out[3] = (val & 0xFF);
  dataToWrite->push_back( pair<int, void*>(4, out) );
  dataToWriteTotalLength += 4;
}

void WriteBuffer::writeLong(unsigned long val){
    
}

void WriteBuffer::writeBool(bool val){
  unsigned char *out = (unsigned char *)malloc(1);
  if (!dataToWrite) dataToWrite = new vector< pair<int, void*> >();
  out[0] = val;
  dataToWrite->push_back( pair<int, void*>(1, out) );
  dataToWriteTotalLength += 1;
}

void WriteBuffer::writeData(void *data, int len){
  char *str = (char*)malloc(len);
  if (!dataToWrite) dataToWrite = new vector< pair<int, void*> >();
  memcpy(str, data, len);
  dataToWrite->push_back( pair<int, void*>(len, str) );
  dataToWriteTotalLength += len;
}

void WriteBuffer::writeString(string &retstr){
  int len = (int)retstr.length();
  char *strptr = (char*)malloc(len);
  writeInt(len);
  memcpy(strptr, retstr.c_str(), retstr.length());
  dataToWrite->push_back( pair<int, void*>(len, (void*)strptr) );
  dataToWriteTotalLength += len;
}

void WriteBuffer::writeStringVector(vector<string> &retstr){
  writeInt((int)retstr.size());
  for (vector<string>::iterator it=retstr.begin();it!=retstr.end(); it++){
    writeString(*it);
  }
}
void WriteBuffer::writeIntVector(vector<int> &retstr){
  writeInt((int)retstr.size());
  for (vector<int>::iterator it=retstr.begin();it!=retstr.end(); it++){
    writeInt(*it);
  }
}

void *WriteBuffer::getDataItem(int itemnum){
  if (itemnum >=0 && itemnum < dataToWrite->size()){
    pair<int, void*> res = (*dataToWrite)[itemnum];
    return res.second;
  }
  return NULL;
}
WriteBuffer::~WriteBuffer(){
    if (dataToWrite || dataToWriteTotalLength){
        cerr << "WARNING:: SocketBuffer::~SocketBuffer: dataToWrite is set: dataToWrite=" << dataToWrite << " dataToWriteTotalLength=" << dataToWriteTotalLength << endl;
        if (dataToWrite){
            for (vector< pair<int, void*> >::iterator it=dataToWrite->begin(); it!=dataToWrite->end(); it++){
                int len = it->first;
                void *data = it->second;
                free(data);
            }
            delete dataToWrite;
            dataToWrite = NULL;
        }
    }    
}
#endif

#ifndef NO_DP
SocketBuffer::~SocketBuffer(){
}
#endif

string getHostName(){
  char hostname[1024];
  hostname[1023] = '\0';
#ifndef _WINDOWS
  gethostname(hostname, 1023);
#endif
  string hn(hostname);
  return hn;
}

long getMilliCount(){
#ifdef _WINDOWS
  return 0l;
#else
	timeb tb;
	ftime(&tb);
	long nCount = tb.millitm + (tb.time & 0xfffff) * 1000;
	return nCount;
#endif
}


void *Base64Decode(string &stringobj, int &len)
{
    unsigned long ixtext, lentext;
    unsigned char ch, inbuf[4], outbuf[3];
    short i, ixinbuf;
    bool flignore, flendtext = false;
    const unsigned char *tempcstring;
    void *theData = NULL;
    int theDataPlace = 0;
    ixtext = 0;
    
    tempcstring = (const unsigned char*)stringobj.c_str();
    
    lentext = stringobj.length();
    
    theData = malloc(lentext);
    
    ixinbuf = 0;
    
    while (true)
    {
        if (ixtext >= lentext)
        {
            break;
        }
        
        ch = tempcstring [ixtext++];
        
        flignore = false;
        
        if ((ch >= 'A') && (ch <= 'Z'))
        {
            ch = ch - 'A';
        }
        else if ((ch >= 'a') && (ch <= 'z'))
        {
            ch = ch - 'a' + 26;
        }
        else if ((ch >= '0') && (ch <= '9'))
        {
            ch = ch - '0' + 52;
        }
        else if (ch == '+')
        {
            ch = 62;
        }
        else if (ch == '=')
        {
            flendtext = true;
        }
        else if (ch == '/')
        {
            ch = 63;
        }
        else
        {
            flignore = true;
        }
        
        if (!flignore)
        {
            short ctcharsinbuf = 3;
            bool flbreak = false;
            
            if (flendtext)
            {
                if (ixinbuf == 0)
                {
                    break;
                }
                
                if ((ixinbuf == 1) || (ixinbuf == 2))
                {
                    ctcharsinbuf = 1;
                }
                else
                {
                    ctcharsinbuf = 2;
                }
                
                ixinbuf = 3;
                
                flbreak = true;
            }
            
            inbuf [ixinbuf++] = ch;
            
            if (ixinbuf == 4)
            {
                ixinbuf = 0;
                
                outbuf[0] = (inbuf[0] << 2) | ((inbuf[1] & 0x30) >> 4);
                outbuf[1] = ((inbuf[1] & 0x0F) << 4) | ((inbuf[2] & 0x3C) >> 2);
                outbuf[2] = ((inbuf[2] & 0x03) << 6) | (inbuf[3] & 0x3F);
                
                for (i = 0; i < ctcharsinbuf; i++)
                {
                    ((unsigned char*)theData)[theDataPlace++] = (unsigned char)outbuf[i];// [theData appendBytes: &outbuf[i] length: 1];
                }
            }
            
            if (flbreak)
            {
                break;
            }
        }
    }
    len = theDataPlace;
    return theData;
}

#ifndef NO_DP
ReadBuffer::ReadBuffer(void *arr, int size, bool rOD) : dataReadTotalLength(0), dataRead(0), dataReadPlace(0), releaseOnDelete(rOD){
    dataRead = (unsigned char *)arr;
    dataReadTotalLength = size;
}

int nreadin = 0;
ReadBuffer::ReadBuffer(int sockfd, int sizen) : dataReadTotalLength(0), dataRead(0), dataReadPlace(0), releaseOnDelete(true){
    int ok = false;
    if (sizen > 0){
        int totalreadin = 0, totalneeded = sizen;
        bool cont = true;
        bool preadin = ((++nreadin) % 1000) == 0;
        dataRead = new unsigned char[sizen];
        while (totalneeded > 0 && cont){
            unsigned char *newpl = dataRead + totalreadin;
#ifdef _WINDOWS
			int readin = recv(sockfd, (char*)newpl, totalneeded, 0);
#else
			int readin = read(sockfd, newpl, totalneeded);
#endif
			if (readin>0){
                totalreadin += readin;
                totalneeded -= readin;
                if (preadin){
                    cout << "  ReadBuffer: readin=" << readin << " bytes, totalling " << (totalreadin) << " sizen=" << sizen;
                    if (totalneeded)
                        cout << " so far, still need " << (totalneeded);
                    cout << endl;
                }
            } else {
                cont = false;
            }
        }
        if (totalneeded==0){
            //      cout << "  readStringFromSocket: returning string with sizen=" << sizen << endl;
            dataReadTotalLength = sizen;
            ok = true;
        }
    } else {
        cout << "WARNING: ReadBuffer instantiated with incorrect number of bytes sizen=" << sizen << endl;
    }
    if (!ok && dataRead){
        delete dataRead;
        dataRead = NULL;
    }
}

ReadBuffer::~ReadBuffer(){
    if (releaseOnDelete && dataRead){
        delete [] ((unsigned char*)dataRead);
        dataRead = NULL;
    }
}

bool ReadBuffer::readBytesDirect(int nbytes, void **bytes){
    if (dataReadTotalLength < dataReadPlace + 1){
        dataReadPlace = dataReadTotalLength;
        return false;
    }
    //unsigned char *ubdest = (unsigned char *)bytes;
    if (dataReadTotalLength < dataReadPlace + nbytes){
        dataReadPlace = dataReadTotalLength;
        return false;
    }
    *bytes = &dataRead[dataReadPlace];
    dataReadPlace += nbytes;
    return true;
}

int ReadBuffer::readBytes(int nbytes, void *bytes){
    if (dataReadTotalLength < dataReadPlace + 1){
        return 0;
    }
    int nb = nbytes, pl = 0, nbfin;
    unsigned char *ubdest = (unsigned char *)bytes;
    nb = nbytes;
    nbfin = nb;
    if (dataReadTotalLength < dataReadPlace + nbytes){
        nb = dataReadTotalLength - dataReadPlace;
    }
//    cout << "readBytes: nbytes=" << nbytes << " nb=" << nb << endl;
    while(nb > 0){
//        cout << "\tdataRead[" << dataReadPlace << "]=" << (int)dataRead[dataReadPlace] << endl;;
        ubdest[pl++] = dataRead[dataReadPlace++];
        nb--;
    }
    return nbfin;
}

int ReadBuffer::readInt(int &val){
  if (!dataRead || dataReadTotalLength < dataReadPlace + 4){
    return 0;
  }
  int ival = ((dataRead[dataReadPlace] << 24) +
	      (dataRead[dataReadPlace+1] << 16) +
	      (dataRead[dataReadPlace+2] << 8) +
	      dataRead[dataReadPlace+3]);
  dataReadPlace += 4;
  val = ival;
  return 4;
}
int ReadBuffer::readString(string &retstr){
  int n = 0, ret;
  ret = readInt(n);
  if (ret){
      readStringWithLength(n, retstr);
      return true;
  }
  return false;
}
void ReadBuffer::readStringWithLength(int n, string &retstr){
    if (n && dataReadTotalLength >= dataReadPlace + n){
        char *ch = (char*)malloc(n);
        memcpy(ch, &dataRead[dataReadPlace], n);
        dataReadPlace += n;
        retstr.assign(ch, n);
        free(ch);
    } else {
        retstr.assign("");
    }
}
int ReadBuffer::skip(int n){
    if (dataReadTotalLength >= dataReadPlace + n){
        dataReadPlace += n;
        return n;
    } else {
        int s = dataReadTotalLength - dataReadPlace;
        if (s > 0){
            dataReadTotalLength = dataReadPlace;
            return s;
        }
    }
    return -1;
}
int ReadBuffer::readStringWithDelim(string &retstr, const char *delimiter, const bool includeDelimiter){
    int pl = dataReadPlace;
    int nmatches = 0;
    auto slen = (int)strlen(delimiter);
    while (nmatches < slen) {
        if (dataRead[pl] == delimiter[nmatches]){
            nmatches++;
        }
        pl++;
        if (pl >= dataReadTotalLength){
            pl = -1;
            break;
        }
    }
    if (pl>0){
        if (includeDelimiter){
            auto len = (int)(pl-dataReadPlace);
            readStringWithLength(len, retstr);
            return len;
        } else {
            auto len = (int)(pl-dataReadPlace - slen);
            readStringWithLength(len, retstr);
            skip(slen);
            return len;
        }
    }
    return -1;
}

int ReadBuffer::readBool(bool &val){
  if (dataReadTotalLength >= dataReadPlace + 1){
    val = dataRead[dataReadPlace++];
    return true;
  }
  return false;
}

#endif

static const char alphanum[] =
       "0123456789"
//        "!@#$%^&*"
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        "abcdefghijklmnopqrstuvwxyz";

float frand(){
    return static_cast <float> (rand()) / static_cast <float> (RAND_MAX);
}
string genRandomString(int len) {
#ifdef _WINDOWS
  char *charStr = (char*)malloc(len + 1);
  for (int i = 0; i < len; ++i) {
    charStr[i] = alphanum[rand() % (sizeof(alphanum) - 1)];
  }
  charStr[len] = 0;
  string str(charStr);
  free(charStr);
  return str;
#else
  char charStr[len+1];
  for (int i = 0; i < len; ++i) {
    charStr[i] = alphanum[rand() % (sizeof(alphanum) - 1)];
  }
  
  charStr[len] = 0;
  return string(charStr);
#endif
}

#ifdef _WINDOWS
#include <winapifamily.h>
#include <iphlpapi.h>
#pragma comment(lib, "iphlpapi.lib")

void getIPFromString(const char *ipString, int *ipInts)
{
	sscanf(ipString, "%d.%d.%d.%d", &ipInts[0], &ipInts[1], &ipInts[2], &ipInts[3]);
}

#define WORKING_BUFFER_SIZE 15000
#define MAX_TRIES 3
#define MALLOC(x) HeapAlloc(GetProcessHeap(), 0, (x))
#define FREE(x) HeapFree(GetProcessHeap(), 0, (x))

void printIPAddressesAvailable(int afInet) {
	DWORD dwSize = 0;
	DWORD dwRetVal = 0;
	ULONG flags = GAA_FLAG_INCLUDE_PREFIX;
	ULONG family = AF_UNSPEC;
	LPVOID lpMsgBuf = NULL;
	PIP_ADAPTER_ADDRESSES pAddresses = NULL;
	ULONG outBufLen = 0;
	ULONG Iterations = 0;
	PIP_ADAPTER_ADDRESSES pCurrAddresses = NULL;
	PIP_ADAPTER_UNICAST_ADDRESS pUnicast = NULL;
	outBufLen = WORKING_BUFFER_SIZE;
	char buff[100] = { 0 };
	DWORD bufflen = 100;
	stringstream ipss;
#ifdef WINAPI_FAMILY_SYSTEM
	ipss << "WINAPI_FAMILY_SYSTEM is set" << endl;
#endif

	family = afInet;
	do {
		pAddresses = (IP_ADAPTER_ADDRESSES *)MALLOC(outBufLen);
		if (pAddresses == NULL) {
			printf
			("Memory allocation failed for IP_ADAPTER_ADDRESSES struct\n");
			exit(1);
		}
		dwRetVal = GetAdaptersAddresses(family, flags, NULL, pAddresses, &outBufLen);
		if (dwRetVal == ERROR_BUFFER_OVERFLOW) {
			FREE(pAddresses);
			pAddresses = NULL;
		}
		else {
			break;
		}
		Iterations++;
	} while ((dwRetVal == ERROR_BUFFER_OVERFLOW) && (Iterations < MAX_TRIES));

	if (dwRetVal == NO_ERROR) {
		pCurrAddresses = pAddresses;
		while (pCurrAddresses) {
			memset(buff, 0, 100);
			bool hasDHCP = false;
			if (pCurrAddresses->Dhcpv4Enabled) {
				if (pCurrAddresses->Dhcpv4Server.iSockaddrLength) {
					bool dhcpv4serverIsSet = false;
					for (int ipb = 0; ipb < pCurrAddresses->Dhcpv4Server.iSockaddrLength && !dhcpv4serverIsSet; ipb++)
						dhcpv4serverIsSet |= pCurrAddresses->Dhcpv4Server.lpSockaddr->sa_data[ipb];
					if (dhcpv4serverIsSet) {
						hasDHCP = true;
					}
				}
			}
			pUnicast = pCurrAddresses->FirstUnicastAddress;
			if (pUnicast) {
				int i;
				for (i = 0; pUnicast != NULL; i++)
				{
					bool isSet = false;
					if (pUnicast->Address.lpSockaddr->sa_family == AF_INET)
					{
						sockaddr_in *sa_in = (sockaddr_in *)pUnicast->Address.lpSockaddr;
						inet_ntop(AF_INET, &(sa_in->sin_addr), buff, bufflen);
						isSet = true;
					}
					else if (pUnicast->Address.lpSockaddr->sa_family == AF_INET6)
					{
						sockaddr_in6 *sa_in6 = (sockaddr_in6 *)pUnicast->Address.lpSockaddr;
						inet_ntop(AF_INET6, &(sa_in6->sin6_addr), buff, bufflen);
						isSet = true;
					}
					pUnicast = pUnicast->Next;
					if (isSet) {
						ipss << "IfType=" << pCurrAddresses->IfType << " : hasDHCP=" << hasDHCP << " ipaddress='" << buff << endl;
						break;
					}
				}
			}
			pCurrAddresses = pCurrAddresses->Next;
		}
	}
	else {
		stringstream ss;
		ss << "Call to GetAdaptersAddresses failed with error: " << dwRetVal << endl;
		dplog(ss.str());
	}
	if (pAddresses) {
		FREE(pAddresses);
	}
	dplog(ipss.str());
}


//windows
string getLocalIPAddressImpl(int afInet) {
	//printIPAddressesAvailable(afInet);
	DWORD dwSize = 0;
	DWORD dwRetVal = 0;
	ULONG flags = GAA_FLAG_INCLUDE_PREFIX;
	ULONG family = AF_UNSPEC;
	LPVOID lpMsgBuf = NULL;
	PIP_ADAPTER_ADDRESSES pAddresses = NULL;
	ULONG outBufLen = 0;
	ULONG Iterations = 0;
	PIP_ADAPTER_ADDRESSES pCurrAddresses = NULL;
	PIP_ADAPTER_UNICAST_ADDRESS pUnicast = NULL;
	outBufLen = WORKING_BUFFER_SIZE;
	char buff[100] = { 0 };
	char tmpbuff[100] = { 0 };
	DWORD bufflen = 100;
	DWORD tmpbufflen = 100;

	family = afInet;
	do {
		pAddresses = (IP_ADAPTER_ADDRESSES *)MALLOC(outBufLen);
		if (pAddresses == NULL) {
			printf
			("Memory allocation failed for IP_ADAPTER_ADDRESSES struct\n");
			exit(1);
		}
		dwRetVal = GetAdaptersAddresses(family, flags, NULL, pAddresses, &outBufLen);
		if (dwRetVal == ERROR_BUFFER_OVERFLOW) {
			FREE(pAddresses);
			pAddresses = NULL;
		}
		else {
			break;
		}
		Iterations++;
	} while ((dwRetVal == ERROR_BUFFER_OVERFLOW) && (Iterations < MAX_TRIES));

	if (dwRetVal == NO_ERROR) {
		pCurrAddresses = pAddresses;
		while (pCurrAddresses) {
			//char sbuf[1024];
			//sprintf(sbuf, "\tFriendly name: %wS type=%d\n", pCurrAddresses->FriendlyName, pCurrAddresses->IfType);
			if (!pCurrAddresses->Dhcpv4Enabled || !pCurrAddresses->Dhcpv4Server.iSockaddrLength) {
				pCurrAddresses = pCurrAddresses->Next;
				continue;
			}
			bool dhcpv4serverIsSet = false;
			for (int ipb = 0; ipb < pCurrAddresses->Dhcpv4Server.iSockaddrLength && !dhcpv4serverIsSet; ipb++)
				dhcpv4serverIsSet |= pCurrAddresses->Dhcpv4Server.lpSockaddr->sa_data[ipb];
			if (!dhcpv4serverIsSet) {
				pCurrAddresses = pCurrAddresses->Next;
				continue;
			}

			int sapl = 0, saplmax = sizeof(pCurrAddresses->Dhcpv4Server.lpSockaddr->sa_data);
			unsigned char firstDHCP = 0;
			while (!firstDHCP && sapl < saplmax) { // for some reason, at least on Hololens Emulator, the IP
								 // address for the DHCP server starts at [2]
				firstDHCP = pCurrAddresses->Dhcpv4Server.lpSockaddr->sa_data[sapl++];
			}
			if (!firstDHCP) {
				pCurrAddresses = pCurrAddresses->Next;
				continue;
			}
			pUnicast = pCurrAddresses->FirstUnicastAddress;
			if (pUnicast) {
				int i;
				for (i = 0; pUnicast != NULL; i++)
				{
					bool isSet = false;
					if (pUnicast->Address.lpSockaddr->sa_family == AF_INET)
					{
						sockaddr_in *sa_in = (sockaddr_in *)pUnicast->Address.lpSockaddr;
						if (((unsigned char*)&sa_in->sin_addr)[0] == firstDHCP) {
							inet_ntop(AF_INET, &(sa_in->sin_addr), tmpbuff, tmpbufflen);
							if (strncmp(tmpbuff, "10.5.5", 6)){ // only check ipv4
							  strcpy(buff, tmpbuff);
							  isSet = true;
							  bufflen = tmpbufflen;
							}
						}
					}
					else if (pUnicast->Address.lpSockaddr->sa_family == AF_INET6)
					{
						sockaddr_in6 *sa_in6 = (sockaddr_in6 *)pUnicast->Address.lpSockaddr;
						inet_ntop(AF_INET6, &(sa_in6->sin6_addr), buff, bufflen);
						isSet = true;
					}
					pUnicast = pUnicast->Next;
					if (isSet) // take first ip address
						break;
				}
			}
			pCurrAddresses = pCurrAddresses->Next;
		}
	}
	else {
		stringstream ss;
		ss << "Call to GetAdaptersAddresses failed with error: " << dwRetVal << endl;
		dplog(ss.str());
	}
	if (pAddresses) {
		FREE(pAddresses);
	}
	return buff;
}

//windows
#ifdef UWP
void ConvertLengthToIpv4Mask(UINT8 PrefixLength, ULONG *mask) {
	*mask = 0l;
	for (int i = 0; i < PrefixLength; i++) {
		*mask |= (1 << i);
	}
}
string getLocalIPBroadcastAddressImpl(int afInet, int sst, int ret) {
	/* Declare and initialize variables */
	DWORD dwSize = 0;
	DWORD dwRetVal = 0;
	char buff[100] = { 0 };
	DWORD bufflen = 100;

	unsigned int i = 0;

	// Set the flags to pass to GetAdaptersAddresses
	ULONG flags = GAA_FLAG_INCLUDE_PREFIX;

	// default to unspecified address family (both)
	ULONG family = AF_UNSPEC;

	LPVOID lpMsgBuf = NULL;

	PIP_ADAPTER_ADDRESSES pAddresses = NULL;
	ULONG outBufLen = 0;
	ULONG Iterations = 0;

	PIP_ADAPTER_ADDRESSES pCurrAddresses = NULL;
	PIP_ADAPTER_UNICAST_ADDRESS pUnicast = NULL;
	PIP_ADAPTER_ANYCAST_ADDRESS pAnycast = NULL;
	PIP_ADAPTER_MULTICAST_ADDRESS pMulticast = NULL;
	IP_ADAPTER_DNS_SERVER_ADDRESS *pDnServer = NULL;
	IP_ADAPTER_PREFIX *pPrefix = NULL;

	family = AF_INET;

	// Allocate a 15 KB buffer to start with.
	outBufLen = WORKING_BUFFER_SIZE;

	do {

		pAddresses = (IP_ADAPTER_ADDRESSES *)MALLOC(outBufLen);
		if (pAddresses == NULL) {
			printf
			("Memory allocation failed for IP_ADAPTER_ADDRESSES struct\n");
			exit(1);
		}

		dwRetVal =
			GetAdaptersAddresses(family, flags, NULL, pAddresses, &outBufLen);

		if (dwRetVal == ERROR_BUFFER_OVERFLOW) {
			FREE(pAddresses);
			pAddresses = NULL;
		}
		else {
			break;
		}

		Iterations++;

	} while ((dwRetVal == ERROR_BUFFER_OVERFLOW) && (Iterations < MAX_TRIES));

	if (dwRetVal == NO_ERROR) {
		// If successful, output some information from the data we received
		pCurrAddresses = pAddresses;
		while (pCurrAddresses) {
			if (!pCurrAddresses->Dhcpv4Enabled) {
				pCurrAddresses = pCurrAddresses->Next;
				continue;
			}
			if (pCurrAddresses->OperStatus != IfOperStatusUp) {
				pCurrAddresses = pCurrAddresses->Next;
				continue;
			}
			pUnicast = pCurrAddresses->FirstUnicastAddress;
			if (pUnicast != NULL) {
				for (i = 0; pUnicast != NULL; i++)
				{
					if (pUnicast->Address.lpSockaddr->sa_family == AF_INET)
					{
						sockaddr_in *sa_in = (sockaddr_in *)pUnicast->Address.lpSockaddr;
						ULONG ip = *(ULONG*)&sa_in->sin_addr;
						ULONG mask;
						ConvertLengthToIpv4Mask(pUnicast->OnLinkPrefixLength, &mask);
						ULONG bcip = ip | ~(mask);
						struct in_addr ia;
						ia.S_un.S_addr = bcip;
						strcpy(buff, inet_ntoa(ia));
						//printf("\tIPV4:%s\n", inet_ntop(AF_INET, &(sa_in->sin_addr), buff, bufflen));
						//printf("\tUDPBroadcast:%s\n", inet_ntop(AF_INET, &(ia), buff, bufflen));
					}
					else if (pUnicast->Address.lpSockaddr->sa_family == AF_INET6)
					{
						sockaddr_in6 *sa_in6 = (sockaddr_in6 *)pUnicast->Address.lpSockaddr;
						printf("\tIPV6:%s\n", inet_ntop(AF_INET6, &(sa_in6->sin6_addr), buff, bufflen));
					}
					else
					{
						printf("\tUNSPEC");
					}
					pUnicast = pUnicast->Next;
				}
			}
			pCurrAddresses = pCurrAddresses->Next;
		}
	}
	else {
		printf("Call to GetAdaptersAddresses failed with error: %d\n",
			dwRetVal);
		if (dwRetVal == ERROR_NO_DATA)
			printf("\tNo addresses were found for the requested parameters\n");
		else {

			if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |
				FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
				NULL, dwRetVal, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
				// Default language
				(LPTSTR) &lpMsgBuf, 0, NULL)) {
				printf("\tError: %s", lpMsgBuf);
				LocalFree(lpMsgBuf);
				if (pAddresses)
					FREE(pAddresses);
				exit(1);
			}
		}
	}
	if (pAddresses) {
		FREE(pAddresses);
	}
	return buff;
}
#else
string getLocalIPBroadcastAddressImpl(int afInet, int sst, int ret) {
	char buff[100] = { 0 };
	
	PIP_ADAPTER_INFO pAdapterInfo;
	PIP_ADAPTER_INFO pAdapter = NULL;
	DWORD dwRetVal = 0;
	UINT i;

	struct tm newtime;
	char buffer[32];
	errno_t error;
	ULONG ulOutBufLen = sizeof(IP_ADAPTER_INFO);
	pAdapterInfo = (IP_ADAPTER_INFO *)MALLOC(sizeof(IP_ADAPTER_INFO));
	if (pAdapterInfo == NULL) {
		printf("Error allocating memory needed to call GetAdaptersinfo\n");
		return "";
	}

	if (GetAdaptersInfo(pAdapterInfo, &ulOutBufLen) == ERROR_BUFFER_OVERFLOW) {
		FREE(pAdapterInfo);
		pAdapterInfo = (IP_ADAPTER_INFO *)MALLOC(ulOutBufLen);
		if (pAdapterInfo == NULL) {
			printf("Error allocating memory needed to call GetAdaptersinfo\n");
			return "";
		}
	}
	if ((dwRetVal = GetAdaptersInfo(pAdapterInfo, &ulOutBufLen)) == NO_ERROR) {
		pAdapter = pAdapterInfo;
		while (pAdapter) {
			if (!pAdapter->DhcpEnabled) {
				pAdapter = pAdapter->Next;
				continue;
			}
			unsigned long ip = inet_addr(pAdapter->IpAddressList.IpAddress.String);
			if (!ip) {
				pAdapter = pAdapter->Next;
				continue;
			}
			if (!strncmp(pAdapter->IpAddressList.IpAddress.String, "10.5.5", 6)) {
				//for gopro
				pAdapter = pAdapter->Next;
				continue;
			}
			unsigned long mask = inet_addr(pAdapter->IpAddressList.IpMask.String);
			unsigned long bcip = ip | ~mask;
			struct in_addr ia;
			ia.S_un.S_addr = bcip;
			strcpy(buff, inet_ntoa(ia));

			pAdapter = pAdapter->Next;
		}
	}
	else {
		printf("GetAdaptersInfo failed with error: %d\n", dwRetVal);

	}
	if (pAdapterInfo)
		FREE(pAdapterInfo);

	return buff;
}
#endif

string getLocalIPAddress() {
	return getLocalIPAddressImpl(AF_INET);
}
string getLocalIP6Address() {
	return getLocalIPAddressImpl(AF_INET6);
}
string getLocalIPBroadcastAddress() {
	return getLocalIPBroadcastAddressImpl(AF_INET, sizeof(struct sockaddr_in), 1);
}

string getLocalIP6Interface() {
	return "";
}
#else
string getLocalIPAddressImpl(int afInet, int sst, int ret);

string getLocalIPAddressImpl(int afInet, int sst, int ret){  // ret 1 - host, 2 - ifa_name
    struct ifaddrs *ifaddr, *ifa;
    int family, s;
    char host[NI_MAXHOST];
    if (getifaddrs(&ifaddr) == -1) {
        cerr << " NetworkComponentPeerServer::bindServer: getifaddrs returns -1" << endl;
        return "";
    }
    
    for (ifa = ifaddr; ifa != NULL; ifa = ifa->ifa_next) {
        if (ifa->ifa_addr == NULL)
            continue;
        family = ifa->ifa_addr->sa_family;
        if (family!=afInet)
            continue;
        string ifa_name = string(ifa->ifa_name);
        std::size_t found = ifa_name.find("lo");
        if (found!=std::string::npos){
            continue;
        }
        if (strncmp(ifa_name.c_str(), "pdp_ip", 6) == 0){ // 3G on IOS
            continue;
        }
        if (family == afInet){
            s = getnameinfo(ifa->ifa_addr,
                            sst,
                            host, NI_MAXHOST, NULL, 0, NI_NUMERICHOST);
            if (s != 0) {
                cout << "getnameinfo() failed: " << gai_strerror(s) << endl;
                continue;
            }
            if (ret==1){
                int ipv4[4];
                sscanf(host, "%d.%d.%d.%d", &ipv4[0], &ipv4[1], &ipv4[2], &ipv4[3]);
                if (ipv4[0] == 10 && ipv4[1] == 5 && ipv4[2] ==5){
                    // gopro network, skip
                    continue;
                }
            }
            //      cout << "\taddress: <" << host << ">\n" << endl;
            freeifaddrs(ifaddr);
            switch (ret){
                case 1:
                    return string(host);
                case 2:
                    return string(ifa_name);
            }
        }
    }
    freeifaddrs(ifaddr);
    return "";
}

string getLocalIPBroadcastAddressImpl(int afInet, int sst, int ret);

string getLocalIPBroadcastAddressImpl(int afInet, int sst, int ret){  // ret 1 - host, 2 - ifa_name
    struct ifaddrs *ifaddr, *ifa;
    int family, s;
    char host[NI_MAXHOST];
    if (getifaddrs(&ifaddr) == -1) {
        cerr << " getLocalIPBroadcastAddressImpl: getifaddrs returns -1" << endl;
        return "";
    }
    
    for (ifa = ifaddr; ifa != NULL; ifa = ifa->ifa_next) {
        
        if (ifa->ifa_dstaddr == NULL)
            continue;
        family = ifa->ifa_dstaddr->sa_family;
        if (family!=afInet)
            continue;
        string ifa_name = string(ifa->ifa_name);
        std::size_t found = ifa_name.find("lo");
        if (found!=std::string::npos){
            continue;
        }
        if (ifa_name == "pdp_ip0"){ // 3G on IOS
            continue;
        }
        if (family == afInet){
            s = getnameinfo(ifa->ifa_dstaddr,
                            sst,
                            host, NI_MAXHOST, NULL, 0, NI_NUMERICHOST);
            if (s != 0) {
                cout << "getnameinfo() failed: " << gai_strerror(s) << endl;
                continue;
            }
            if (ret==1){
                int ipv4[4];
                sscanf(host, "%d.%d.%d.%d", &ipv4[0], &ipv4[1], &ipv4[2], &ipv4[3]);
                if (ipv4[0] == 10 && ipv4[1] == 5 && ipv4[2] ==5){
                    // gopro network, skip
                    continue;
                }
            }
            //      cout << "\taddress: <" << host << ">\n" << endl;
            freeifaddrs(ifaddr);
            switch (ret){
                case 1:
                    return string(host);
                case 2:
                    return string(ifa_name);
            }
        }
    }
    freeifaddrs(ifaddr);
    return "";
}

string getAndCheckAllNetworkInterfacesImpl(int afInet, int sst, int ret);

string getAndCheckAllNetworkInterfacesImpl(int afInet, int sst, int ret){  // ret 1 - host, 2 - ifa_name
    struct ifaddrs *ifaddr, *ifa;
    int family, s;
    char host[NI_MAXHOST];
    if (getifaddrs(&ifaddr) == -1) {
        cerr << " NetworkComponentPeerServer::bindServer: getifaddrs returns -1" << endl;
        return "";
    }
    cout << "getLocalIPAddressImpl: return: " << (ret == 1 ? "host" : "ifa_name") << endl;
    for (ifa = ifaddr; ifa != NULL; ifa = ifa->ifa_next) {
        if (!ifa->ifa_name){
            continue;
        }
        string ifa_name = string(ifa->ifa_name);
        cout << "  ifa_name: " << ifa_name << endl;
        if (ifa->ifa_addr == NULL)
            continue;
        family = ifa->ifa_addr->sa_family;
        if (family!=afInet)
            continue;
        std::size_t found = ifa_name.find("lo");
        if (found!=std::string::npos){
            continue;
        }
        if (strncmp(ifa_name.c_str(), "pdp_ip", 6) == 0){ // 3G on IOS
            continue;
        }
        if (family == afInet){
            s = getnameinfo(ifa->ifa_addr,
                            sst,
                            host, NI_MAXHOST, NULL, 0, NI_NUMERICHOST);
            if (s != 0) {
                cout << "getnameinfo() failed: " << gai_strerror(s) << endl;
                continue;
            }
            if (ret==1){
                int ipv4[4];
                sscanf(host, "%d.%d.%d.%d", &ipv4[0], &ipv4[1], &ipv4[2], &ipv4[3]);
                cout << "     host: '" << host << "'" << endl;
                if (ipv4[0] == 10 && ipv4[1] == 5 && ipv4[2] ==5){
                    // gopro network, skip
                    continue;
                }
            }
            //      cout << "\taddress: <" << host << ">\n" << endl;
            freeifaddrs(ifaddr);
            switch (ret){
                case 1:
                    return string(host);
                case 2:
                    return string(ifa_name);
            }
        }
    }
    freeifaddrs(ifaddr);
    return "";
}

string getLocalIPAddress(){
    return getLocalIPAddressImpl(AF_INET, sizeof(struct sockaddr_in), 1);
}
string getLocalIPBroadcastAddress(){
    return getLocalIPBroadcastAddressImpl(AF_INET, sizeof(struct sockaddr_in), 1);
}

string getLocalIP6Address(){
    return getLocalIPAddressImpl(AF_INET6, sizeof(struct sockaddr_in6), 1);
}

string getLocalIP6Interface(){
    return getLocalIPAddressImpl(AF_INET6, sizeof(struct sockaddr_in6), 2);
}
string getAllLocalIPAddresses();
string getAllLocalIPAddresses(){
    return getAndCheckAllNetworkInterfacesImpl(AF_INET, sizeof(struct sockaddr_in), 1);
}

#endif

double dpmin(double first, double second, bool ret[]){
    if (first > second){
        ret[0] = true;
        return (second);
    } else {
        return (first);
    }
}

double dpmax(double first, double second, bool ret[]){
    if (first < second){
        ret[0] = true;
        return (second);
    } else {
        return (first);
    }
}

void *convertStringsToContiguousNullDelimitedArray(const vector<string> &lstr){
    int totlen = 0;
    for (auto it=lstr.begin();it!=lstr.end(); it++){
        totlen += (*it).size();
    }
    void *ret = malloc(totlen + lstr.size() + 4);
    char *retCH = (char*)(ret) + 4;
    ((int*)ret)[0] = lstr.size();
    int pl = 0;
    for (auto it=lstr.begin();it!=lstr.end(); it++){
        const char *strptr = (*it).c_str();
        int slen = (int)strlen(strptr);
        memcpy(&retCH[pl], strptr, slen);
        pl += slen + 1;
        retCH[pl-1] = 0;
    }
    retCH[pl] = 0;
    return ret;
}

#ifdef _WINDOWS
void usleep(__int64 usec)
{
	HANDLE timer;
	LARGE_INTEGER ft;

	ft.QuadPart = -(10 * usec); // Convert to 100 nanosecond interval, negative value indicates relative time

	timer = CreateWaitableTimer(NULL, TRUE, NULL);
	SetWaitableTimer(timer, &ft, 0, NULL, NULL, 0);
	WaitForSingleObject(timer, INFINITE);
	CloseHandle(timer);
}
void sleep(int usec) {
	usleep(1000l * usec);
}
#endif

int genRandomInt(){
#if defined(_OSX) || defined(IOS)
    int r = arc4random_uniform(INT_MAX);
#else
    uint64_t randomnum =
    (((uint64_t)rand() << 0) & 0x000000000000FFFFull) |
    (((uint64_t)rand() << 16) & 0x00000000FFFF0000ull) |
    (((uint64_t)rand() << 32) & 0x0000FFFF00000000ull) |
    (((uint64_t)rand() << 48) & 0xFFFF000000000000ull);
    int r = (int)(randomnum % INT_MAX);
#endif
    return r;
}

const string urlencode(const string input) {
    stringstream output;
    auto sourceLen = input.length();
    for (int i = 0; i < sourceLen; ++i) {
        const unsigned char thisChar = input[i];
        if (thisChar == ' '){
            output << "+";
        } else if (thisChar == '.' || thisChar == '-' || thisChar == '_' || thisChar == '~' ||
                   (thisChar >= 'a' && thisChar <= 'z') ||
                   (thisChar >= 'A' && thisChar <= 'Z') ||
                   (thisChar >= '0' && thisChar <= '9')) {
            output << thisChar;
        } else {
            output << '%' << setfill('0') << setw(2) << right << hex << thisChar;
        }
    }
    return output.str();
}
