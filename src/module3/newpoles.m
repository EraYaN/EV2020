s=ss11;
poles=[-1.38084 -1.38084];
Lpoles=[-20 -20];

K=acker(s.a,s.b,poles);
A=s.a-s.b*K;
B=s.b;
C=s.c;
D=s.d;
Nbar=rscale(A,B,C,D,K);
L=acker(A',C',Lpoles)';

sim('modelerwin');