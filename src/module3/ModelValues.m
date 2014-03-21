%A = [0,1;-0.296800000000000,-1.782000000000000];
%B = [5.639000000000000;-39.940000000000000];
%C = [1,0];

L_poles = [-4, -4]; %[-4, -4]
K_poles = [-2, -2]; %[-1.38084, -1.38084]

L = acker(ss11.a', ss11.c', L_poles);
K = acker(ss11.a, ss11.b, K_poles);

A = ss11.a-ss11.b*K;
B = ss11.b;
C = ss11.c;
D = 0;

nbar = rscale(A, B, C, D, K);

sim('model2');