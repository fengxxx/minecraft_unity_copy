#mccc.py


from pycraft import anvil

#a=anvil.open("region.mca")
# a=anvil.open("r.2.-2.mca")
a=anvil.open("r.1.1.mca")

#//print a.indexs



f=open("world.1.1.txt",'w')


aa=""
zz=""
for s in range(0,1024):
	c= a.load_chunk(s)

	if c!=None:
			#if s==1:
			# print s,c
			#print s,type(c),c

			# print c.keys()
			# print c[u'Level'].keys()
			# print c[u'Level'][u"xPos"]
			# print c[u'Level'][u"zPos"]
			# # print c[u'Level'][u"yPos"]
			# print len(c[u'Level'][u'Sections'])
			# print c[u'Level'][u'Sections'][0][u'Y']
			# print c[u'Level'][u'Sections'][0][u'Blocks']

			ac=""
			count=0
			for b in c[u'Level'][u'Sections'][0][u'Blocks']:
				
				x=count%16%16
				z=count/(16*16)
				y=(count/16)%16
				#print (count,x,y,z)
				count+=1
				if count==4096:
					ac+=str(b)
				else:
					
					ac+=str(b)+" "
			zz+=str(c)
	aa+=str(s)+"|"+ac+"\n"
	
#print aa

f.write(zz)

f.close()

# print len(c[u'Level'][u'Sections'][0][u'Blocks'])

# print dir(a.load_chunk(1))
# meta = anvil.Metadata(2 ** 32 - 1, 2 ** 32 - 1)

# for offset in range(2, 18):
#     for length in range(16):
#         for timestamp in range(2 ** 31, 2 ** 31 + 16):
#             meta.offset = offset
#             meta.length = length
#             meta.timestamp = timestamp
#             print meta.location
#             # self.assertEqual(offset, meta.offset)
#             # self.assertEqual(length, meta.length)
#             # self.assertEqual(timestamp, meta.timestamp)
