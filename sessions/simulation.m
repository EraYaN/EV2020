% x=[0;0];
% xdot=[0;0];
% u=0;
% y=0;
% output=zeros(300,1);
% sys=idss(A,B,C,D,K,[0 0],0.1);
% [output, Tsim, outputx]=lsim(ss11,input);
%sys=ss(A,B,C,D);
s=ss11d;
samples = 200;
time=linspace(0,0.1*(samples-1),samples);
%s2=ss11dcmCopy;
%Nbar = rscale(ss11d,K);
[output, Tsim, outputx]=lsim(s,-1.6*norm.u,time);
%output2=lsim(s2,-20*norm.u,time);
% for i=1:300,
%     u=input(i);
%     xdot=A*x+B*u;
%     y=C*x;
%     x=x+xdot;
%     output(i)=y;
% end
plot(time,[norm.u output norm.y]);
axis([0 30 -250 250])
legend('input','output', 'data');
pole(s)
pole(s2)
zero(s)
zero(s2)