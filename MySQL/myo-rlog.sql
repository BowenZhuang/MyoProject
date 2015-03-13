create database myo;
use myo;

create table rlog (
	rid Integer not null auto_increment primary key,
	rdate varchar(30) not null,
	rdata1 integer not null,
	rdata2 integer not null,
	rdata3 integer not null,
	rdata4 integer not null,
	rdata5 integer not null,
	rdata6 integer not null,
	rdata7 integer not null,
	rdata8 integer not null
)engine = MyIsam;

create table rlog1 (
	rid Integer not null auto_increment primary key,
	rdate varchar(30) not null,
	rdata1 integer not null,
	rdata2 integer not null,
	rdata3 integer not null,
	rdata4 integer not null,
	rdata5 integer not null,
	rdata6 integer not null,
	rdata7 integer not null,
	rdata8 integer not null
)engine = MyIsam;