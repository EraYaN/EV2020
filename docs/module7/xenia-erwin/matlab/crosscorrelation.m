clear all
close all;
clc;

N=100;
L=4;
h=[3 1 2 -4];

t=zeros(1,N);
for i=1:N
    t(i)=i;
end

%minimum-phase sequence
x_1 = [1 ; -0.5];
X_1 = [x_1; zeros((N-length(x_1)),1)];

%maximum-phase sequence
x_2 = [1;-2];
X_2 = [x_2; zeros((N-length(x_2)),1)];

%sinusoidal signal
%omega=0.2
omega = 0.2;
X_3 = [zeros(N,1)];
for i = 1:(N-1)
    X_3(i) = cos(omega*i);
end

%random BPSK sequence
X_4 = sign(randn(N,1));

%convolution
%y_1 = conv(X_1,h);
%y_2 = conv(X_2,h);
%y_3 = conv(X_3,h);
%y_4 = conv(X_4,h);

Xmat=[X_1, X_2,X_3,X_4];

%% Kruiscorrelatie
for i=1:4
    figure;
    number=zeros(1,4);
    tel=0;
        for j=1:4
                
                if i~=j
                    tel=tel+1;
                    number(tel)=j
                    r= zeros(2*N-1,4);
                    r(:,j)=conv( Xmat(:,i),flipud(Xmat(:,j)'));
                    hold on;
                    plot(r);              
                end
        end
            title(['Kruiscorrelatie x',num2str(i),' met: '])
            legend([num2str(number(1))], [num2str(number(2))], [num2str(number(3))], [num2str(number(4))]);
end