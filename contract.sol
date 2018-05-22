pragma solidity ^0.4.23;

contract FileTransaction{
 
    struct File{
        uint id;
        string filehash;
        string backuphash;
        string date;
    }
    
   mapping(address => mapping(uint => File)) private mapfiles;
   mapping(address => uint) private FileCount;
   
   function AddFiles(uint _id, string _fileHash, string _backuphash, string _date) public 
   {
       File memory tmpfile = File(_id, _fileHash, _backuphash, _date);
       FileCount[msg.sender]++;
       mapfiles[msg.sender][FileCount[msg.sender]] = tmpfile;

   }
   
   function GetFiles(uint _count) public view returns(uint id, string filehash, string backuphash, string date){
         return (mapfiles[msg.sender][_count].id, mapfiles[msg.sender][_count].filehash, mapfiles[msg.sender][_count].backuphash, mapfiles[msg.sender][_count].date);
   }
   
   function GetCount() public view returns(uint count){
         return FileCount[msg.sender];
   }
   
}