//# show_panel:LivingRoom

ช่วงสายของวันถัดมาในขณะที่คุณกำลังพักผ่อนอยู่ในห้องนั่งเล่น #VOICE:บรรยาย-23  # show_panel:LivingRoom # hide_panel:PhoneCalling # hide_panel:PhoneCallIn

-> show_callin
== show_callin ==
//# play_bgm:ringtone
# show_panel:PhoneCallIn 
คุณได้กดวางสายโทรศัพท์  
# hide_panel:PhoneCallIn  
//# stop_bgm
-> call
//--> END

== call ==
"สวัสดีครับ ผมพลตำรวจโท ทวียศ" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-13 # show_panel:PhoneCalling # hide_panel:LivingRoom
"เจ้าหน้าที่กองปราบปรามยาเสพติด" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-14 # show_panel:PhoneCalling # hide_panel:LivingRoom
"ขอเรียนสายคุณน้ำ แม่ของผู้ต้องหา" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-15 # show_panel:PhoneCalling # hide_panel:LivingRoom
"ลูกชายของคุณถูกจับเนื่องจากมีสารเสพติดไว้ ในครอบครอง" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-16 # show_panel:PhoneCalling # hide_panel:LivingRoom
"และจากการสอบสวน เรายังพบว่ามีสารเสพติด บางส่วน ถูกส่งไปที่บ้านของคุณ" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-17 # show_panel:PhoneCalling # hide_panel:LivingRoom
"ขณะนี้ลูกชายของคุณถูกควบคุมตัวไว้ในห้องขัง    ชั่วคราว" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-18 # show_panel:PhoneCalling # hide_panel:LivingRoom
  *[คุณตกใจกับสิ่งที่ได้ยิน] "เรื่องจริงเหรอคะ!? ขอคุยกับลูกชายหน่อยได้ไหม" #speaker:คุณ #VOICE:พากย์-19 # show_panel:PhoneCalling # hide_panel:LivingRoom
"ทางเราไม่สามารถดำเนินการให้ได้" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-20 # show_panel:PhoneCalling # hide_panel:LivingRoom
"เนื่องจากลูกชายของคุณกำลังอยู่ใต้ฤทธิ์ของ   สารเสพติด" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-21 # show_panel:PhoneCalling # hide_panel:LivingRoom

//# hide_panel:PhoneCallIn

ตำรวจโทรมาบอกคุณเรื่องของลูกชาย และถามถึงเรื่องการประกันตัว #VOICE:บรรยาย-31 # show_panel:PhoneCalling # hide_panel:LivingRoom
-> choices

== choices ==
*[อยากจะช่วยลูกชาย]-> Scene4_2
*[ขอตรวจสอบความจริงก่อน]-> Scene4_3
*[ไม่เชื่อและตัดสายทิ้ง]-> Scene4_4

== Scene4_2 ==
//# show_panel:PhoneCalling
"ถ้าจะช่วยลูกชาย ต้องทำยังไงบ้างคะ" #speaker:คุณ #VOICE:พากย์-22 # show_panel:PhoneCalling # hide_panel:LivingRoom
"ตามหลักกฎหมายการมียาเสพติดไว้ในการครอบ ครอง" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-23 # show_panel:PhoneCalling # hide_panel:LivingRoom
"มีโทษจำคุก 2 ปีหรือต้องจ่ายค่าประกันตัว 40,000 บาท"  #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-24 # show_panel:PhoneCalling # hide_panel:LivingRoom # hide_panel:Son
"โปรดแอดไลน์ ID ตามที่ผมบอก" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-25 # show_panel:PhoneCalling # hide_panel:LivingRoom
"จากนั้นส่งรูปบัตรประชาชนของคุณแม่มาด้วยครับ จะได้ไม่ต้องมาที่สน. ทางเราจะดำเนินการให้" #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-26 # show_panel:PhoneCalling # hide_panel:LivingRoom # hide_panel:Son

ตำรวจให้แอดไลน์ ID เพื่อส่งรูปบัตรประชาชนและบอกค่าประกันตัว ลูกชายที่สูงถึง 40,000 บาท #VOICE:บรรยาย-32 # show_panel:PhoneCalling # hide_panel:LivingRoom # hide_panel:Son
	-> choice2

== choice2==
 *[ดำเนินการตามที่ตำรวจบอก] -> BadEnd1
 *[ค่าปรับสูง ขอเวลาในการตรวจสอบให้มั่นใจ] -> Scene4_3

== Scene4_3 ==
//# play_bgm:ringtone
# hide_panel:PhoneCalling
คุณให้ตำรวจถือสายรอ และรีบเดินไปยังกล่องพัสดุที่ส่งมาเมื่อวาน เพื่อที่จะตรวจสอบว่าข้างในมีอะไรอยู่ # play_sound:OpenBox #VOICE:บรรยาย-24 # show_panel:Box # hide_panel:Son

คุณเปิดออกมาและได้พบกับสารสีขาวบางอย่าง ที่อยู่ในซองพลาสติก #VOICE:บรรยาย-25 # show_panel:Powder #hide_panel:Box # hide_panel:Son

อีกทั้งจำนวนของซองพลาสติกที่อยู่ในกล่องก็มี จำนวนอยู่ไม่น้อยเลยทีเดียว #VOICE:บรรยาย-26 # hide_panel:Son
//# hide_panel:Powder
  *[จ่ายค่าปรับให้ลูกชาย] -> BadEnd1 
  *[ปรึกษาคนใกล้ตัว]  -> goodEnd2
  *[ปล่อยลูกชายติดคุก]  -> AlternativeEnd3


== Scene4_4 ==
คุณตัดสายของคนที่อ้างว่าตัวเองเป็นตำรวจทิ้งและ กลับไปทำสิ่งที่คุณสนใจต่อ #VOICE:บรรยาย-27 # hide_panel:PhoneCallIn # hide_panel:PhoneCalling # show_panel:LivingRoom

จนเวลาได้ผ่านไปพักหนึ่ง... #VOICE:บรรยาย-28  # hide_panel:PhoneCalling # hide_panel:LivingRoom # show_panel:PhoneCallIn 

-> contiScene4_4  
// # show_panel:PhoneCallIn # play_bgm:ringtone
// คุณตัดสายของคนที่อ้างว่าตัวเองเป็นตำรวจทิ้งและกลับไปทำสิ่งที่คุณสนใจต่อ
// จนเวลาได้ผ่านไปได้พักหนึ่ง..
//  *[รับสาย] -> contiScene4_4 
//# hide_panel:PhoneCallIn # stop_bgm
//  *[ตัดสาย] ->Scene4_4 

== contiScene4_4 ==
# show_panel:PhoneCalling # hide_panel:LivingRoom
"สวัสดีครับคุณน้ำ ผมพลตำรวจโท ทวียศนะครับ"  #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-27
"ผมเข้าใจว่าคุณแม่อาจจะไม่เชื่อ แต่ผมโทรมาอีกครั้งเพื่อยืนยัน"  #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-28
"ว่าลูกชายของคุณแม่กำลังถูกฝากขังอยู่ที่สน.ด้วย ข้อหาเกี่ยวกับเรื่องยาเสพติด"  #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-29
"หากคุณแม่ไม่เชื่อ ลองเปิดพัสดุล่าสุดที่ส่งมาถึงบ้านคุณแม่ดูครับ"  #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-30
"เพราะในระหว่างที่คุณแม่ตัดสายไป ทางเราได้สอบสวนลูกชายของคุณแม่เพิ่มเติม"  #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-31 # hide_panel:Son
"เราจำเป็นต้องดำเนินการตามขั้นตอนของกฎหมาย"  #speaker:พลตำรวจโท ทวียศ #VOICE:พากย์-32 # hide_panel:Son # show_panel:PhoneCalling

ตำรวจโทรมาย้ำคุณเรื่องของลูกชายและให้คุณ เลือกประกันตัวหรือปล่อยให้ลูกชายนอนคุก #VOICE:บรรยาย-29 # hide_panel:Son # show_panel:PhoneCalling
-> choice3

== choice3==
 *[ลองตรวจสอบพัสดุ] -> Scene4_3
 *[จ่ายค่าประกันตัว] -> BadEnd1
 *[ปล่อยลูกชายติดคุก] -> AlternativeEnd3

== BadEnd1 == 

คุณทำตามที่ตำรวจบอกและโอนเงินค่าประกัน ตำรวจจึงขอเวลาในการดำเนินการและวางสายคุณ ไป #VOICE:จบ1-0 # hide_panel:PhoneCalling # hide_panel:Box # hide_panel:Powder # show_panel:Son # hide_panel:LivingRoom # hide_panel:Police


ภายในเย็นวันนั้น คุณที่ได้เป็นห่วงลูกชายอย่างมากจึงโทรไปหา #VOICE:จบ1-1 # hide_panel:PhoneCalling # hide_panel:Box # hide_panel:Powder # show_panel:Son # hide_panel:LivingRoom # hide_panel:Police
คุณถามถึงเรื่องคดี ว่ามีความเป็นมาอย่างไร #VOICE:จบ1-2 # hide_panel:PhoneCalling # hide_panel:Box # hide_panel:Powder # show_panel:Son # hide_panel:LivingRoom # hide_panel:Police
แต่เขากลับมีท่าทีสับสนกับคำพูดของคุณ #VOICE:จบ1-3 

เขาเตือนสติคุณและบอกให้คุณรีบไปแจ้งความ โดยตรง  #VOICE:จบ1-4  

คุณได้ลงบันทึกประจำวันเอาไว้ #VOICE:จบ1-6 # show_panel:Police 

ตำรวจบอกกับคุณว่าคงไม่สามารถช่วยอะไรได้มาก และยังบอกอีกด้วยว่าคุณโชคดีที่เสียเงินไปแค่ส่วนหนึ่ง #VOICE:จบ1-7 

ถ้าหากมิจฉาชีพใช้วิธีการอื่นที่สามารถเข้าถึง แอพธนาคารคุณได้ คุณอาจสูญเสียเงินในบัญชี ทั้งหมดไปเลย #VOICE:จบ1-8 

# show_panel:Ending1
# hide_panel:Son
# hide_panel:Police 
//# hide_panel:Son


-> END

== goodEnd2 ==
//# hide_panel:Box # hide_panel:Powder # show_panel:Son

คุณตัดสินใจเอาเรื่องราวที่เจอไปลองปรึกษาเพื่อน และญาติ ทุกเสียงต่างก็บอกเหมือนกันว่าเป็นเรื่อง หลอกลวง #VOICE:จบ2-0 # hide_panel:PhoneCalling # hide_panel:Box # hide_panel:Powder # show_panel:Son # hide_panel:LivingRoom # hide_panel:Police
ลูกชายของคุณที่พวกเขารู้จักเป็นเด็กดี ขยันขันแข็ง ไม่มีทางที่เขาจะเข้าไปยุ่งกับยาเสพติดอย่างแน่นอน #VOICE:จบ2-1 # hide_panel:Police # show_panel:Son

คุณจึงลองโทรศัพท์ไปหาลูกชาย และลูกชายก็ได้รับสาย #VOICE:จบ2-2 # hide_panel:Police # show_panel:Son

คุณระบายเรื่องที่ได้เจอมาในวันนี้ด้วยความโล่งใจ #VOICE:จบ2-3 # hide_panel:Police # show_panel:Son
ลูกชายก็ชื่นชมคุณที่คุณไม่ตกเป็นเหยื่อของ แก๊งมิจฉาชีพและได้กล่าวถึงพัสดุ ที่ส่งไปยังบ้านคุณเมื่อวาน #VOICE:จบ2-4 # hide_panel:Police # show_panel:Son

มันไม่ใช่พัสดุที่เขาสั่งแต่อย่างใดเพราะเขาได้ตรวจ  สอบแล้วว่าพัสดุที่เขาได้สั่งเอาไว้ยังส่งไปไม่ถึงคุณ #VOICE:จบ2-5 

ก่อนลูกชายของคุณจะวางสาย เขาได้แนะนำให้ เอาพัสดุปริศนาไปแจ้งความ #VOICE:จบ2-6 

ภายในเย็นวันนั้นคุณก็ได้ลงบันทึกประจำวันถึง เรื่องราวที่เกิดขึ้น และนำพัสดุไปให้ตำรวจได้ตรวจสอบ #VOICE:จบ2-7 # show_panel:Police # hide_panel:Son

ไม่กี่วันต่อมาตำรวจก็ได้ติดต่อคุณกลับ และได้ บอกถึงสิ่งที่อยู่ในกล่องพัสดุนั้นว่าแท้จริงแล้วมัน ก็เป็นแค่ "แป้ง" #VOICE:จบ2-8 

# show_panel:Ending2 
# hide_panel:Police
-> END

== AlternativeEnd3 ==

ตำรวจตกตะลึงไปพักนึงกับการตัดสินใจของคุณ #VOICE:จบ3-0 # hide_panel:Powder # show_panel:PhoneCalling # hide_panel:Son
เขารับเรื่องและบอกจะดำเนินการให้ก่อนที่จะวางสายไป #VOICE:จบ3-1 # show_panel:Son # hide_panel:PhoneCalling 


คุณได้แต่เสียใจในสิ่งลูกชายได้ทำลงไป และยังเสียใจกับการที่ต้องปล่อยให้ลูกแท้ ๆ ของตัวเองต้องนอนคุก #VOICE:จบ3-2 # show_panel:Son # hide_panel:PhoneCalling 

นี่คงเป็นบทเรียนที่มีค่าสำหรับเขา และหวังว่าเมื่อลูกชายของคุณออกจากคุกแล้ว เขาจะเป็นคนที่ดีขึ้นและไม่เข้าไปยุ่งกับสารเสพติดอีก #VOICE:จบ3-3 # show_panel:Son # hide_panel:PhoneCalling  # hide_panel:BedRoom
//# play_bgm:morning
ในเช้าวันถัดมา คุณยังคงค้างคากับเรื่องที่เกิดขึ้น เมื่อวาน แต่แล้วก็ได้มีเสียงข้อความแจ้งเตือนมา จากโทรศัพท์ของคุณในช่วงเช้าเหมือนทุกที #VOICE:จบ3-4 # hide_panel:Box # hide_panel:Son # show_panel:BedRoom

คุณได้แต่สงสัยเพราะข้อความเหล่านั้นส่งมาจาก ลูกชายของคุณ #VOICE:จบ3-6 # hide_panel:Box # hide_panel:Son # show_panel:BedRoom

ข้อความที่เขาส่งมา ก็เป็นการทักทายแบบปกติ เหมือนในทุก ๆ วัน #VOICE:จบ3-7 # hide_panel:Box # hide_panel:Son # show_panel:BedRoom

คุณรีบโทรถามลูกชายเรื่องการถูกจับ #VOICE:จบ3-8 # hide_panel:Box # hide_panel:Son # show_panel:BedRoom

ลูกชายก็ได้ตอบกลับด้วยท่าทีที่งุนงง และวีดิโอ คอลเพื่อยืนยันว่าเขายังอยู่ดีแล้วกำลังจะเข้าทำงาน #VOICE:จบ3-9 # hide_panel:Box # hide_panel:Son # show_panel:BedRoom

วันนั้นทั้งวันคุณก็ได้แต่สับสน ประมวลผลถึง เหตุการณ์ที่เกิดขึ้นจนจบวัน #VOICE:จบ3-10
# show_panel:Ending3
# hide_panel:Box # hide_panel:Son
# hide_panel:BedRoom 

-> END
