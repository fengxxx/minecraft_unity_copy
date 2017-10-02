#mccc.py


from pycraft import anvil
import os,sys,struct
#a=anvil.open("region.mca")
# a=anvil.open("r.2.-2.mca")
#a=anvil.open("r.1.1.mca")
#//print a.indexs

#print sys.platform
dirName="F:\\Minecraft1.8\\.minecraft\\saves\\Rhion\\region\\"
if sys.platform=="darwin":
 	dirName="/Users/fengx/Desktop/last/last/Minecraft_server_backup_15:7:30/Rhion/region/"
# print( dirName)

def translateMCA(filename):
	print(filename)
	saveFilename=os.path.splitext(filename)[0]+".txt"
	print (saveFilename)
	f=open(saveFilename,'wb')
	strs=""

	bs=b''
	a=anvil.open(filename)
	for s in range(0,1024):
		#print(s)
		c= a.load_chunk(s)
		#print (c)
		# ac=""
		
		if c!=None:
			bs+=struct.pack('i',s)
			bs+=struct.pack('i', len(c[u'Level'][u'Sections']))
			
			bs+=struct.pack('256i',*c[u'Level'][u"HeightMap"]) 
			for sec in  c[u'Level'][u'Sections']:
				# count=0
				# for b in sec[u'Blocks']:
				# 	# x=count%16%16
				# 	# z=count/(16*16)
				# 	# y=(count/16)%16
				# 	# count+=1
				# 	# if count==4096:
				# 	# 	ac+=str(b)
				# 	# else:
				# 	#ac+=str(b)+" "
				# 	ac+=struct.pack('i',b)
				# # strs+=str(s)+"|"+ac+"\n"
				# for ii in range(0,4096):
				# 	print(sec[u'Blocks'][ii])
				# 	print(chr(sec[u'Blocks'][ii]))
				# 	#print((sec[u'Blocks'][ii]))
				# 	bs+=struct.pack('C',chr(sec[u'Blocks'][ii]))
				bs+=struct.pack('4096i',*sec[u'Blocks'])
				#bs+=struct.pack('i',sec[u'Blocks'][0])
				
				# return 0;
	f.write(bs)
	f.close()
	print("done: "+filename)
count=0
for f in os.listdir(dirName):
	

	if os.path.splitext(f)[1]==".mca" and count<1:
		translateMCA(dirName+f)
	elif os.path.split(f)[1]=="r.0.-1.mca":
		translateMCA(dirName+f)
	count+=1
	



# f=open("world.1.1.txt",'w')

# aa=""
# zz=""
# for s in range(0,1024):
# 	c= a.load_chunk(s)
# 	ac=""
# 	if c!=None:
# 			#if s==1:
# 			# print s,c
# 			#print s,type(c),c

# 			# print c.keys()
# 			# print c[u'Level'].keys()
# 			# print c[u'Level'][u"xPos"]
# 			# print c[u'Level'][u"zPos"]
# 			# # print c[u'Level'][u"yPos"]
# 			# print len(c[u'Level'][u'Sections'])
# 			# print c[u'Level'][u'Sections'][0][u'Y']
# 			# print c[u'Level'][u'Sections'][0][u'Blocks']

# 			#ac=""
# 			count=0
# 			for b in c[u'Level'][u'Sections'][0][u'Blocks']:
				
# 				x=count%16%16
# 				z=count/(16*16)
# 				y=(count/16)%16
# 				#print (count,x,y,z)
# 				count+=1

# 				if count==4096:
# 					ac+=str(b)
# 				else:
					
# 					ac+=str(b)+" "

# 			zz+=str(c)

# 	aa+=str(s)+"|"+ac+"\n"
	
# #print aa

# f.write(aa)

# f.close()

# # print len(c[u'Level'][u'Sections'][0][u'Blocks'])

# # print dir(a.load_chunk(1))
# # meta = anvil.Metadata(2 ** 32 - 1, 2 ** 32 - 1)

# # for offset in range(2, 18):
# #     for length in range(16):
# #         for timestamp in range(2 ** 31, 2 ** 31 + 16):
# #             meta.offset = offset
# #             meta.length = length
# #             meta.timestamp = timestamp
# #             print meta.location
# #             # self.assertEqual(offset, meta.offset)
# #             # self.assertEqual(length, meta.length)
# #             # self.assertEqual(timestamp, meta.timestamp)