clc; clear;
close all;
tol=0.01;
Zin=[26.5-10.4i 73.6+35.8i 39.1-29.3i];
f=[9.65e9 9.85e9 10e9];
mi = 0;
ma = 200;
n=ma-mi;
ZL = 100; % Ohm Load impedance
Z0 = 50; % Ohm 
c0=3e8;
e_r = 2.205;
lambda = c0 ./ (f .* sqrt(e_r));
beta = 2*pi./lambda;
c=1./beta.*(atan(i*(Z0.*Zin-ZL.*Z0)./(Zin.*ZL-Z0^2)));
L=zeros(3,n);
% make k array
k=linspace(mi,ma-1,n);
for j=1:3;
    L(j,1:n)=real(c(j)+k.*pi/beta(j));
end
hold all;
plot(k,L);
diff = zeros(n,n,n);
for j1=1:n;
    for j2=1:n;
        for j3=1:n;
            % fill difference matrix
            a1=abs(L(1,j1) - L(2,j2));
            a2=abs(L(2,j2) - L(3,j3));
            a3=abs(L(1,j1) - L(3,j3));
            diff(j1,j2,j3) = max([a1 a2 a3]);
        end
    end
end
%find lowest difference between the lines
[value, index] = min(diff(:));
if(value<tol)
    % convert index to matrix indices
    [p,q,s] = ind2sub(size(diff), index);
    % take the average of the two lengths
    length = (real(L(1,p))+real(L(2,p))+real(L(3,s)))/3;
    % print result
    fprintf('Found solution @ iteration %i*%i*%i and value = %f, dist = %f\n', p, q, s, length, diff(p,q,s));  
else
    disp('No solution');
end