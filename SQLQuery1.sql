CREATE TABLE [dbo].[ManaulReview_DataSet_20170719](
 [ID] [int] IDENTITY(1,1) NOT NULL,
 [Costar_controlnumber] [varchar](50) NULL,
 [costarpath] [varchar](max) NULL,
 [xceligentpath] [varchar](500) NULL,
 [Similar] [varchar](500) NULL,
 [Manual_QC_Similar] [bit] NULL,
 [Manual_Comments] [varchar](max) NULL,
 [Manual_QC_By] [varchar](50) NULL,
 [DateLogged_QC] [datetime] NULL
) ON [PRIMARY];

select [ID],[Costar_controlnumber],[costarpath],[xceligentpath],[Similar],[Manual_QC_Similar],[Manual_Comments],[Manual_QC_By],[DateLogged_QC] from [ManaulReview_DataSet_20170719]

select [costarpath] from [ManaulReview_DataSet_20170719]
where [DateLogged_QC] is null
group by [costarpath];

insert into [ManaulReview_DataSet_20170719]
([Costar_controlnumber],[costarpath],[xceligentpath],[Similar],[Manual_Comments],[Manual_QC_By],[DateLogged_QC]) values ('COSTAR0002978','C:\Users\Ashraf\AppData\Roaming\Skype\My Skype Received Files\Issues\Cropped\6176255.jpg','C:\Users\Ashraf\AppData\Roaming\Skype\My Skype Received Files\Issues\Original\COSTAR0000292.jpg','No','my comment2222','Ashraf Kamal .Qssass',GETDATE())

insert into [ManaulReview_DataSet_20170719]
([Costar_controlnumber],[costarpath],[xceligentpath],[Similar],[Manual_Comments],[DateLogged_QC]) values ('COSTAR0002978','C:\Users\Ashraf\AppData\Roaming\Skype\My Skype Received Files\Issues\Cropped\6176255.jpg','C:\Users\Ashraf\AppData\Roaming\Skype\My Skype Received Files\Issues\Original\AshrafKamal_CV.pdf','No','new new commnet',GETDATE())


insert into [ManaulReview_DataSet_20170719]
([Costar_controlnumber],[costarpath],[xceligentpath],[Similar]) values ('COSTAR0002978','C:\Users\Ashraf\Desktop\COSTAR0005232.jpg','C:\Users\Ashraf\Desktop\10070556-0015.jpg','No');

insert into [ManaulReview_DataSet_20170719]
([Costar_controlnumber],[costarpath],[xceligentpath],[Similar]) values ('COSTAR0002978','C:\Users\Ashraf\AppData\Roaming\Skype\My Skype Received Files\Issues\Cropped\5189820.jpg','C:\Users\Ashraf\AppData\Roaming\Skype\My Skype Received Files\Issues\Original\COSTAR0000292.jpg','Yes')

select [costarpath],
count(0) total,
(select count(0) from [ManaulReview_DataSet_20170719] where x.[costarpath]=[costarpath] and [DateLogged_QC] is not null) processed
 from [ManaulReview_DataSet_20170719] x
   -- where [DateLogged_QC] is not null
     group by[costarpath]
	