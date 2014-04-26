%f1=10000; % Hz
n=100;
points=n*10;
ZL = 100; % Ohm
Z0 = 50;
Zin=[26.5-i*10.4 73.6+i*35.8 39.1-i*29.3];
f=[9.65e9 9.85e9 10e9];
%C0 = 0.0001 % F
%L0 = 0.0001 % H
%R0 = 0.1;
cmed=3e8; % m/s
%Zin1=80; % Ohm
w = 2.*pi.*f;
beta=w./cmed;
%Gamma1=(ZL-Z0)/(ZL+Z0);

L=zeros(3,n);
L2=zeros(3,n);
k=linspace(0,n-1,n*10);
for j=1:3;
    L(j,1:points)=1./beta(j).*(atan((ZL - Zin(j))/(Z0*i - (ZL*Zin(j)*i)/Z0))+k.*pi);
    
    L2(j,1:points)=1./beta(j).*(atan(i*(Z0.*Zin(j)-ZL.*Z0)./(Zin(j)-Z0))+k.*pi);
end
%L(1,1:points)=1./beta(1).*(atan(i*(Z0.*Zin(1)-ZL.*Zin(1))./(ZL.*Z0-Z0.^2))+k.*pi);
%L(2,1:points)=1./beta(2).*(atan(i*(Z0.*Zin(2)-ZL.*Zin(2))./(ZL.*Z0-Z0.^2))+k.*pi);
%L(3,1:points)=1./beta(3).*(atan(i*(Z0.*Zin(3)-ZL.*Zin(3))./(ZL.*Z0-Z0.^2))+k.*pi);
subplot(2,1,1);
plot(k,L);
axis([0 n 0 1.6])
subplot(2,1,2);
plot(k,L2);
axis([0 n 0 1.6])



