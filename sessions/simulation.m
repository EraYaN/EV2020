% x=[0;0];
% xdot=[0;0];
% u=0;
% y=0;
% output=zeros(300,1);
% sys=idss(A,B,C,D,K,[0 0],0.1);
% [output, Tsim, outputx]=lsim(ss11,input);
%sys=ss(A,B,C,D);
[output, Tsim, outputx]=lsim(ss11,input,time);
% for i=1:300,
%     u=input(i);
%     xdot=A*x+B*u;
%     y=C*x;
%     x=x+xdot;
%     output(i)=y;
% end
plot(time,[input output outputx data]);
axis([0 30 -400 400])
legend('input','output','outputx1','outputx2','data');
pole(sys)
zero(sys)