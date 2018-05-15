pragma solidity ^0.4.23;

contract FileTransaction{
 
    struct File{
        uint id;
        string filehash;
        string date;
    }
    
   mapping(address => File[]) public mapfiles;
   mapping(address => uint) public FileCount;
   
   function AddFiles(uint _id, string _fileHash, string _date) public 
   {
       mapfiles[msg.sender].push(File(_id,_fileHash,_date));
       FileCount[msg.sender]++;
   }
   
   function GetFiles(uint _count) public view returns(uint id, string filehash,string date){
         return (mapfiles[msg.sender][_count].id, mapfiles[msg.sender][_count].filehash,  mapfiles[msg.sender][_count].date);
   }
   
}