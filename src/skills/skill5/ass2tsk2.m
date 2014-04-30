clear;
close all;
disp('Started');
tic
%define input impedances and frequencies
Zin = [26.5-10.4i 73.6+35.8i 39.1+29.3i];
f = [9.65e9 9.85e9 10e9];
% min and max iterations max needed is 295
mi = 0;
ma = 295;
% number of points
n=ma-mi;
% impedance paramters
ZL = 100; % Ohm Load impedance
Z0 = 50; % Ohm 
% calculating beta (http://www.antenna-theory.com/definitions/permittivity.php)
c0=3e8;
e_r = 2.205;
lambda = c0 ./ (f .* sqrt(e_r));
beta = 2*pi./lambda;
% calculate constant term
c=1./beta.*(atan(1i*(Z0.*Zin-ZL.*Z0)./(Zin.*ZL-Z0^2)));
% initialize L matrix
L=zeros(3,n);
% make k array
k=linspace(mi,ma-1,n);
% calculate values in the L matrix
for j=1:3;
    L(j,1:n)=real(c(j)+k.*pi/beta(j));
end
% define the tolerance (the no solution boundary)
tol=0.00001;
% execute the bruteforce algorithm
diff=get_distance(n, L, tol);
toc
disp('Done');