poles=[-2.24385 -2.24385];
Lpoles=[-4 -4];
K=acker(A_old2,B,poles);
A=A_old2-B*K;
Nbar=rscale(A,B,C,D,K);
L=acker(A',C',Lpoles)';